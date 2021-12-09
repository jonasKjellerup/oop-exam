using oop_exam.UI;

namespace oop_exam
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			var stregsystem = new Stregsystem();
			var ui = new StregsystemCLI(stregsystem);
			var controller = new StregsystemController(ui, stregsystem);
			
			ui.Start();
		}
	}
}