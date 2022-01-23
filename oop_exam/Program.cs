using oop_exam.UI;

namespace oop_exam
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			using var stregsystem = new Stregsystem();
			var ui = new StregsystemCLI(stregsystem);
			var _ = new StregsystemController(ui, stregsystem);
			
			ui.Start();
		}
	}
}