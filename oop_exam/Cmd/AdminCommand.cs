using System.Collections.Generic;

namespace oop_exam.Cmd
{
    public class AdminCommand : ICommand
    {
        public string Name { get; }

        public string[] Parameters { get; }

        public AdminCommand(string name, string[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }

        public void Execute(StregsystemController controller)
        {
            var command = controller.GetAdminCommand(Name);
            if (command is null)
            {
                controller.Ui.DisplayAdminCommandNotFoundMessage(Name);
                return;
            }

            command(Parameters);
        }
    }
}