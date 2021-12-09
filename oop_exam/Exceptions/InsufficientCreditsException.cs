using System;
using oop_exam.Models;

namespace oop_exam.Exceptions
{
	public class InsufficientCreditsException : Exception
	{
		public User User;
		public Product Product;

		public override string Message => $"{User.Username} has insufficient funds to purchase {Product.Name}.";
		
		public InsufficientCreditsException(User user, Product product)
		{
			User = user;
			Product = product;
		}

	}
}