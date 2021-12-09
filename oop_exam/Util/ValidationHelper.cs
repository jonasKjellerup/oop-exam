using System;
using System.Text.RegularExpressions;

namespace oop_exam.Util
{
	public static class ValidationHelper
	{
		private static readonly Regex UsernameRegex = new("^[0-9a-z_]+$");
		private static readonly Regex EmailRegex = new("^[0-9a-z_.-]+@(?<host>[0-9a-z][0-9a-z.-]*[0-9a-z])$");
		private static readonly Regex HtmlTagRegex = new("<[^>]+>");
		
		public static void NullCheck<T>(T value, string name) where T: class
		{
			if (value == null)
				throw new ArgumentNullException($"Argument {name} must be non null.");
		}

		public static void LengthCheck(string value, string name, uint minLength, uint maxLength = uint.MaxValue)
		{
			if (value.Length <= minLength || value.Length >= maxLength)
				throw new ArgumentException($"String {name} must have minimum length {minLength} and maximum length {maxLength}");
		}

		public static string StripHtmlTags(string input)
		{
			return HtmlTagRegex.Replace(input, "");
		}
		
		public static string ValidateEmail(string email)
		{
			email = email.ToLower(); // TODO argue why
			var match = EmailRegex.Match(email);
			
			if (!match.Success || !match.Groups["host"].Captures[0].Value.Contains('.'))
				throw new ArgumentException("Invalid email given.");

			return email;
		}

		public static void CheckUsername(string name)
		{
			if (!UsernameRegex.IsMatch(name))
				throw new ArgumentException("Invalid username given.");
		}
	}
}