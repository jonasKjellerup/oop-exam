using oop_exam.Exceptions;

namespace oop_exam.Models
{
	public class BuyTransaction : Transaction
	{
		public Product Product { get; }
		
		public BuyTransaction(Product product, User user) : base(user, product.Price)
		{
			Product = product;
		}

		public override void Execute()
		{
			if (!Product.CanBeBoughtOnCredit && User.Balance < Product.Price)
				throw new InsufficientCreditsException(User, Product);
			User.Balance -= Amount;
		}
	}
}