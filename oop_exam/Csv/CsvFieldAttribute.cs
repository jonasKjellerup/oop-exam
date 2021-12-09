using System;

namespace oop_exam.Csv
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvFieldAttribute : Attribute
    {
        public CsvFieldAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}