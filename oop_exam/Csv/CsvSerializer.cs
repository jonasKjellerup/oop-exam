using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using oop_exam.Exceptions;

namespace oop_exam.Csv
{
    public class CsvSerializer
    {
        private enum ReadState
        {
            ValueEnd = 0,
            RowEnd,
            StreamEnd,
        }
        
        private readonly List<byte> _workBuffer = new();


        /// <summary>
        /// Converts the work buffer to a utf-8 string without first
        /// copying to an array (<c>Encoding.GetString</c> does not accept lists).
        /// </summary>
        /// <returns>A utf-8 encoded string.</returns>
        private string WorkBufferToUtf8()
        {
            return Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(_workBuffer));
        }

        private static PropertyInfo[] GetMarkedProperties(Type t)
        {
            var markedProperties = t.GetProperties()
                .Where(p => p.GetCustomAttribute<CsvFieldAttribute>() is not null)
                .ToArray();

            if (!markedProperties.Any())
                throw new Exception($"Type {t.Name} has no properties marked as csv fields.");

            // We only support serialization of primitive, decimal, and string values.
            // This ensures that all of the selected properties are match these criteria. 
            if (markedProperties.Any(p => !p.PropertyType.IsPrimitive && p.PropertyType != typeof(string) && p.PropertyType != typeof(decimal)))
                throw new CsvSerializationException("Can not serialize csv value to non primitive/string/decimal type.");

            return markedProperties;
        }
        
        private (ReadState, string) ReadValue(Stream stream, char separator)
        {
            try
            {
                var quoted = false;
                do
                {
                    var b = stream.ReadByte();
                    if (b == -1)
                    {
                        if (_workBuffer.Count != 0)
                            throw new CsvSerializationException("Unexpected end of input mid value");
                        return (ReadState.StreamEnd, "");
                    }

                    if (b == separator || b == '\n')
                    {
                        if (quoted)
                            throw new CsvSerializationException(
                                $"Expected `\"` before `{separator}` got `{(char) b}`");
                        var state = b == '\n' ? ReadState.RowEnd : ReadState.ValueEnd;
                        var value = WorkBufferToUtf8();
                        return (state, value);
                    }

                    if (b == '"')
                    {
                        if (_workBuffer.Count == 0)
                        {
                            quoted = true;
                            continue;
                        }

                        if (!quoted)
                            throw new CsvSerializationException("Unexpected `\"` character");

                        var next = stream.ReadByte();
                        if (next == -1)
                            throw new CsvSerializationException(
                                "Unexpected end of input stream following closing quotation mark." +
                                "\nNote that all rows containing values should be ended by a line break.");
                        var state = (char) next switch
                        {
                            '\n' => ReadState.RowEnd,
                            _ when next == separator => ReadState.ValueEnd,
                            _ => throw new Exception($"Expected either `\n` or `{separator}` following closing `\"`")
                        };

                        return (state, WorkBufferToUtf8());
                    }

                    _workBuffer.Add((byte) b);
                } while (true);
            }
            finally
            {
                _workBuffer.Clear();
            }
        }

        public List<T> Deserialize<T>(Stream src, char separator)
        {
            _workBuffer.Clear();
            
            // Get type metadata through reflection
            var targetType = typeof(T);
            var markedProperties = GetMarkedProperties(targetType);

            // Read the title row (the first row)
            var csvColumnNames = new List<string>();

            ReadState state;
            string value;

            do
            {
                (state, value) = ReadValue(src, separator);
                if (state == ReadState.StreamEnd)
                    throw new CsvSerializationException("Unexpected end of stream while parsing title row");

                csvColumnNames.Add(value);
            } while (state != ReadState.RowEnd);

            // Map columns to properties by title
            var columnProperties = new PropertyInfo?[csvColumnNames.Count]; 
            
            {
                var propertyNameMap = new Dictionary<string, PropertyInfo>(markedProperties.Select(
                    p => new KeyValuePair<string, PropertyInfo>(
                        p.GetCustomAttribute<CsvFieldAttribute>()!.Name,
                        p
                    )
                ));

                foreach (var (columnName, i) in csvColumnNames.Select((n,i) => (n,i)))
                {
                    if (propertyNameMap.TryGetValue(columnName, out var property))
                        columnProperties[i] = property;
                }
            }

            // Parse each row as a instance of T
            var rows = new List<T>();
            var instance = FormatterServices.GetUninitializedObject(targetType);
            var column = 0;
            
            (state, value) = ReadValue(src, separator);
            while (state != ReadState.StreamEnd)
            {
                var targetProperty = columnProperties[column++];
                if (targetProperty is not null)
                {
                    object finalValue = value;
                    if (targetProperty.PropertyType != typeof(string))
                        finalValue = Convert.ChangeType(value, targetProperty.PropertyType);

                    targetProperty.SetValue(instance, finalValue);
                }

                if (state == ReadState.RowEnd)
                {
                    if (column != csvColumnNames.Count)
                        throw new CsvSerializationException("Inconsistent csv format: too few values in row");
                    
                    rows.Add((T)instance);
                    
                    instance = FormatterServices.GetUninitializedObject(targetType);
                    column = 0;
                } else if (column >= csvColumnNames.Count)
                    throw new CsvSerializationException("Inconsistent csv format: too many values in row");

                (state, value) = ReadValue(src, separator);
            }

            return rows;
        }

        public static void SerializeColumnHeaderTo<T>(Stream dst, char separator)
        {
            var writer = new StreamWriter(dst);
            var properties = GetMarkedProperties(typeof(T));
            foreach (var property in properties)
            {
                var name = property.GetCustomAttribute<CsvFieldAttribute>()!.Name;
                writer.Write(name);
                writer.Write(separator);
            }
            
            writer.Flush();
            
            dst.Position -= 1;
            dst.WriteByte((byte)'\n');
        }

        public static void SerializeRowTo<T>(Stream dst, char separator, T rowValue)
        {
            var writer = new StreamWriter(dst);
            var properties = GetMarkedProperties(typeof(T));
            foreach (var property in properties)
            {
                var value = property.GetValue(rowValue);
                if (value is null)
                    throw new CsvSerializationException($"Unable to serialize property {property.Name}. Value is null.");
                
                writer.Write(value.ToString());
                writer.Write(separator);
            }
            
            writer.Flush();
            dst.Position -= 1;
            dst.WriteByte((byte)'\n');
        }
        
        public string Serialize<T>(IEnumerable<T> src, char separator)
        {
            _workBuffer.Clear();
            
            // Get type metadata through reflection
            var targetType = typeof(T);
            var markedProperties = GetMarkedProperties(targetType);

            for (var i = 0; i < markedProperties.Length; i++)
            {
                if (i > 0)
                    _workBuffer.Add((byte)separator);
                var property = markedProperties[i];
                _workBuffer.AddRange(Encoding.UTF8.GetBytes(property.GetCustomAttribute<CsvFieldAttribute>()!.Name));
            }
            _workBuffer.Add((byte)'\n');

            foreach (var obj in src)
            {
                for (var i = 0; i < markedProperties.Length; i++)
                {
                    if (i > 0) 
                        _workBuffer.Add((byte)separator);
                    var value = markedProperties[i].GetValue(obj);
                    if (value is null)
                        throw new CsvSerializationException($"Unable to serialize property {markedProperties[i].Name}. Value is null.");
                    
                    _workBuffer.AddRange(Encoding.UTF8.GetBytes(value.ToString()!));
                }
                _workBuffer.Add((byte) '\n');
            }

            return WorkBufferToUtf8();
        }
    }
}