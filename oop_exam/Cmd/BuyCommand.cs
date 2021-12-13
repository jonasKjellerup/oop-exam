using System.Diagnostics;
using oop_exam.Models;

namespace oop_exam.Cmd
{
    public class BuyCommand : UserCommand
    {
        private uint ProductId { get; }
        private uint Count { get; }

        public BuyCommand(string username, uint productId, uint count) : base(username)
        {
            ProductId = productId;
            Count = count;
        }

        public override void Execute(StregsystemController controller)
        {
            var stregsystem = controller.Stregsystem;
            var ui = controller.Ui;

            if (Count == 0)
            {
                ui.DisplayGeneralError("Buy count was 0. Nothing was bought.");
                return;
            }

            var user = GetUser(stregsystem, ui);
            if (user is null)
                return;

            if (!stregsystem.TryGetProductById(ProductId, out var product))
            {
                ui.DisplayProductNotFound(ProductId);
                return;
            }

            var totalSum = product.Price * Count;
            if (totalSum > user.Balance)
            {
                ui.DisplayInsufficientCash(user, product, Count);
                return;
            }

            var transaction = new BuyTransaction(product, user);
            for (var i = Count; i > 0; i--)
                stregsystem.ExecuteTransaction(transaction);

            ui.DisplayUserBuysProduct(Count, transaction);
        }
    }
}