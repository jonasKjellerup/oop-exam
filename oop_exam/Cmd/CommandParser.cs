using System;
using System.Linq;
using oop_exam.Util;

namespace oop_exam.Cmd
{
	public static class CommandParser
	{

		private static BuyCommand BuildBuyCommand(string[] strings)
		{
			uint buyCount = 1;

			if (!uint.TryParse(strings[1], out var productId))
				throw new ArgumentException("Non positive integer input given as buy command argument");

			if (strings.Length > 2)
			{
				buyCount = productId;
				if (!uint.TryParse(strings[2], out productId))
					throw new ArgumentException("Non positive integer input given as buy command argument");
			}

			return new BuyCommand(strings[0], productId, buyCount);
		}
		
		public static ICommand Parse(string input)
		{
			ValidationHelper.LengthCheck(input, nameof(input), 1);

			var strings =  input
				.Split(' ')
				.Where(s => !string.IsNullOrWhiteSpace(s)) // Remove empty strings
				.ToArray();

			return strings[0][0] switch
			{
				':' => new AdminCommand(strings[0], strings[1..]),
				_ when ValidationHelper.IsValidUsername(strings[0]) is false 
					=> throw new ArgumentException("Invalid command input. Expected valid username or admin command."),
				_ when strings.Length > 1 => BuildBuyCommand(strings),
				_ => new UserCommand(strings[0])
			};

		}
	}
}