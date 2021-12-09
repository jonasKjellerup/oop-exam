namespace oop_exam.Command
{
	public class Command
	{
		public string Name { get; }
		public string[] Args { get; }

		public Command(string name, string[] args)
		{
			Name = name;
			Args = args;
		}
	}
}