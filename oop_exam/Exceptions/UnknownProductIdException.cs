using System;

namespace oop_exam.Exceptions
{
	public class UnknownProductIdException : Exception
	{
		public readonly uint Id;
		
		public UnknownProductIdException(uint id) : base($"No product with id {id} was found")
		{
			Id = id;
		}
	}
}