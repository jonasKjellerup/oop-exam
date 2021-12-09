using System;

namespace oop_exam.Exceptions
{
    public class CsvSerializationException : Exception
    {
        public CsvSerializationException(string msg) : base(msg){}
    }
}