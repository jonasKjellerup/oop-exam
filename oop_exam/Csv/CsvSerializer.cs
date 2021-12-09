using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

        private readonly char _separator;
        private readonly List<byte> _workBuffer = new();

        public CsvSerializer(char separator)
        {
            _separator = separator;
        }

        /// <summary>
        /// Converts the work buffer to a utf-8 string without first
        /// copying to an array (<c>Encoding.GetString</c> does not accept lists).
        /// </summary>
        /// <returns>A utf-8 encoded string.</returns>
        private string WorkBufferToUtf8()
        {
            return Encoding.UTF8.GetString(CollectionsMarshal.AsSpan(_workBuffer));
        }

        private (ReadState, string) ReadValue(Stream stream)
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

                    if (b == _separator || b == '\n')
                    {
                        if (quoted)
                            throw new CsvSerializationException(
                                $"Expected `\"` before `{_separator}` got `{(char) b}`");
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
                            _ when next == _separator => ReadState.ValueEnd,
                            _ => throw new Exception($"Expected either `\n` or `{_separator}` following closing `\"`")
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

        public List<T> Deserialize<T>(Stream src)
        {
            // Get type metadata through reflection
            var targetType = typeof(T);
            var markedProperties = targetType
                .GetProperties()
                .Where(p => p.GetCustomAttribute<CsvFieldAttribute>() is not null)
                .ToArray();

            if (!markedProperties.Any())
                throw new Exception($"Type {targetType.Name} has no properties marked as csv fields.");

            // We only support deserialization of primitive, decimal, and string values.
            // This ensures that all of the selected properties are match these criteria. 
            if (markedProperties.Any(p => !p.PropertyType.IsPrimitive && p.PropertyType != typeof(string) && p.PropertyType != typeof(decimal)))
                throw new CsvSerializationException("Can not serialize csv value to non primitive/string/decimal type.");

            // Read the title row (the first row)
            var csvColumnNames = new List<string>();

            ReadState state;
            string value;

            do
            {
                (state, value) = ReadValue(src);
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
            var instance = Activator.CreateInstance(targetType)!;
            var column = 0;
            
            (state, value) = ReadValue(src);
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
                    
                    instance = Activator.CreateInstance(targetType)!;
                    column = 0;
                } else if (column >= csvColumnNames.Count)
                    throw new CsvSerializationException("Inconsistent csv format: too many values in row");

                (state, value) = ReadValue(src);
            }

            return rows;
        }
    }
}