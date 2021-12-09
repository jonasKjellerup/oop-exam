using oop_exam.UI;

namespace oop_exam
{
	public class StregsystemController
	{
		private IStregsystemUI _ui;
		private IStregsystem _stregsystem;

		public StregsystemController(IStregsystemUI ui, IStregsystem stregsystem)
		{
			_ui = ui;
			_stregsystem = stregsystem;
		}
	}
}