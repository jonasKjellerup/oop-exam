using System;

namespace oop_exam.Exceptions
{
	public class UnknownUsernameException : Exception
	{
		public readonly string Username;

		public UnknownUsernameException(string name) : base($"Unable to find user {name}") 
		{
			Username = name;
		}
	}
}