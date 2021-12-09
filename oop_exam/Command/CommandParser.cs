using System.Collections.Generic;
using oop_exam.Util;

namespace oop_exam.Command
{
	public static class CommandParser
	{
		public static Command Parse(string input)
		{
			ValidationHelper.NullCheck(input, nameof(input));
			ValidationHelper.LengthCheck(input, nameof(input), 1);
			
			var isAdminCommand = input[0] == ':';
			var sliceStart = 0 + (isAdminCommand ? 1 : 0);
			var cursor = sliceStart;

			List<string> parts = new List<string>();
			
			while (cursor < input.Length)
			{
				switch (input[cursor])
				{
					// End non-quoted string
					case ' ':
						// If string is non empty
						if (cursor > sliceStart)
							parts.Add(input[sliceStart..cursor]);

						// Move slice start to next cursor position
						sliceStart += 1;
						break;

					// If end of line is reached while reading a string
					case '\n' when cursor > sliceStart:
						parts.Add(input[sliceStart..cursor]);
						break;
				}

				cursor += 1;
			}

			return new Command("a", new []{"test"});

		}
	}
}