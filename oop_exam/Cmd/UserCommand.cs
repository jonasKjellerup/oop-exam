using oop_exam.Models;
using oop_exam.UI;

namespace oop_exam.Cmd
{
    public class UserCommand : ICommand
    {
        private string Username { get; }

        public UserCommand(string username)
        {
            Username = username;
        }

        protected User? GetUser(IStregsystem stregsystem, IStregsystemUI ui)
        {
            if (stregsystem.TryGetUserByUsername(Username, out var user))
                return user;

            ui.DisplayUserNotFound(Username);
            return null;
            
        }

        public virtual void Execute(StregsystemController controller)
        {
            var ui = controller.Ui;
            var user = GetUser(controller.Stregsystem, ui);
            if (user is not null)
                ui.DisplayUserInfo(user);
        }
    }
}