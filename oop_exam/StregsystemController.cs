using System;
using System.Collections.Generic;
using oop_exam.Cmd;
using oop_exam.UI;

namespace oop_exam
{
    public class StregsystemController
    {
        private readonly Dictionary<string, Action<string[]>> _adminCommands;

        public IStregsystem Stregsystem { get; }
        public IStregsystemUI Ui { get; }

        public StregsystemController(IStregsystemUI ui, IStregsystem stregsystem)
        {
            Ui = ui;
            Stregsystem = stregsystem;
            Ui.CommandEntered += OnUserInput;

            _adminCommands = new Dictionary<string, Action<string[]>>
            {
                {":q", Quit},
                {":quit", Quit},
                {":activate", args => SetProductActivationState(true, args)},
                {":deactivate", args => SetProductActivationState(false, args)},
                {":crediton", args => SetCreditBuyAbility(true, args)},
                {":creditoff", args => SetCreditBuyAbility(false, args)},
                {":addcredits", AddCreditToUser}
            };
        }


        public Action<string[]>? GetAdminCommand(string name)
            => _adminCommands.TryGetValue(name, out var action) ? action : null;

        private void Quit(IEnumerable<string> _) => Ui.Close();

        private void SetProductActivationState(bool state, string[] args)
        {
            if (args.Length > 1)
            {
                Ui.DisplayTooManyArgumentsError(":activate/:deactivate");
                return;
            }

            if (!uint.TryParse(args[0], out var productId))
            {
                Ui.DisplayGeneralError("Expected product id to be a non-negative integer value.");
                return;
            }

            if (!Stregsystem.TryGetProductById(productId, out var product))
            {
                Ui.DisplayProductNotFound(productId);
                return;
            }
            
            product.Active = state;
        }

        private void SetCreditBuyAbility(bool state, string[] args)
        {
            if (args.Length > 1)
            {
                Ui.DisplayTooManyArgumentsError(":crediton/:creditoff");
                return;
            }

            if (!uint.TryParse(args[0], out var productId))
            {
                Ui.DisplayGeneralError("Expected product id to be a non-negative integer value.");
                return;
            }

            if (!Stregsystem.TryGetProductById(productId, out var product))
            {
                Ui.DisplayProductNotFound(productId);
                return;
            }
            
            product.CanBeBoughtOnCredit = state;
        }

        private void AddCreditToUser(string[] args)
        {
            if (args.Length > 2)
            {
                Ui.DisplayTooManyArgumentsError(":addcredits");
                return;
            }

            if (!Stregsystem.TryGetUserByUsername(args[0], out var user))
            {
                Ui.DisplayUserNotFound(args[0]);
                return;
            }

            if (!decimal.TryParse(args[1], out var amount))
            {
                Ui.DisplayGeneralError("Second argument must be a valid number.");
                return;
            }
            
            Stregsystem.AddCreditsToAccount(user, amount);
        }

        public void OnUserInput(string input)
        {
            try
            {
                var command = CommandParser.Parse(input);
                command.Execute(this);
            }
            catch (Exception e)
            {
                Ui.DisplayGeneralError(e.Message);
            }
        }
    }
}