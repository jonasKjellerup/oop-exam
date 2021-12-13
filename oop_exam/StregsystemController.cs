using System;
using System.Collections.Generic;
using oop_exam.Cmd;
using oop_exam.UI;

namespace oop_exam
{
    public class StregsystemController
    {
        private readonly IStregsystemUI _ui;
        private readonly IStregsystem _stregsystem;
        private readonly Dictionary<string, Action<string[]>> _adminCommands;

        public IStregsystem Stregsystem => _stregsystem;
        public IStregsystemUI Ui => _ui;

        public StregsystemController(IStregsystemUI ui, IStregsystem stregsystem)
        {
            _ui = ui;
            _stregsystem = stregsystem;
            _ui.CommandEntered += OnUserInput;

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

        private void Quit(IEnumerable<string> _) => _ui.Close();

        private void SetProductActivationState(bool state, string[] args)
        {
            if (args.Length > 1)
            {
                _ui.DisplayTooManyArgumentsError(":activate/:deactivate");
                return;
            }

            if (!uint.TryParse(args[0], out var productId))
            {
                _ui.DisplayGeneralError("Expected product id to be a non-negative integer value.");
                return;
            }

            if (!_stregsystem.TryGetProductById(productId, out var product))
            {
                _ui.DisplayProductNotFound(productId);
                return;
            }
            
            product.Active = state;
        }

        private void SetCreditBuyAbility(bool state, string[] args)
        {
            if (args.Length > 1)
            {
                _ui.DisplayTooManyArgumentsError(":crediton/:creditoff");
                return;
            }

            if (!uint.TryParse(args[0], out var productId))
            {
                _ui.DisplayGeneralError("Expected product id to be a non-negative integer value.");
                return;
            }

            if (!_stregsystem.TryGetProductById(productId, out var product))
            {
                _ui.DisplayProductNotFound(productId);
                return;
            }
            
            product.CanBeBoughtOnCredit = state;
        }

        private void AddCreditToUser(string[] args)
        {
            if (args.Length > 2)
            {
                _ui.DisplayTooManyArgumentsError(":addcredits");
                return;
            }

            if (!_stregsystem.TryGetUserByUsername(args[0], out var user))
            {
                _ui.DisplayUserNotFound(args[0]);
                return;
            }

            if (!decimal.TryParse(args[1], out var amount))
            {
                _ui.DisplayGeneralError("Second argument must be a valid number.");
                return;
            }
            
            _stregsystem.AddCreditsToAccount(user, amount);
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
                _ui.DisplayGeneralError(e.ToString());
            }
        }
    }
}