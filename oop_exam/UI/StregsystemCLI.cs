using System;
using oop_exam.Models;

namespace oop_exam.UI
{
	public class StregsystemCLI : IStregsystemUI
	{
		private bool _running;
		private IStregsystem _stregsystem;

		public StregsystemCLI(IStregsystem stregsystem)
		{
			_stregsystem = stregsystem;
		}

		public void DisplayUserNotFound(string username)
		{
			Console.WriteLine($"User {username} not found.");
		}

		public void DisplayProductNotFound(uint product)
		{
			Console.WriteLine($"Product by id: {product} not found.");
		}

		public void DisplayUserInfo(User user)
		{
			Console.WriteLine($"\nUsername: {user.Username}");
			Console.WriteLine($"User id: {user.Id}");
			Console.WriteLine($"Fullname: {user.Firstname} {user.Lastname}");
			Console.WriteLine($"Email: {user.Email}");
			Console.WriteLine($"Current balance: {user.Balance}");
		}

		public void DisplayTooManyArgumentsError(string command)
		{
			Console.WriteLine($"Too many arguments given for command {command}");
		}

		public void DisplayAdminCommandNotFoundMessage(string adminCommand)
		{
			Console.WriteLine($"No no such admin command as {adminCommand}");
		}

		public void DisplayUserBuysProduct(BuyTransaction transaction)
		{
			Console.WriteLine($"{transaction.User.Username} has bought {transaction.Product.Name} for {transaction.Amount}");
		}

		public void DisplayUserBuysProduct(uint count, BuyTransaction transaction)
		{
			Console.WriteLine($"{transaction.User.Username} has bought {count}x {transaction.Product.Name} for {transaction.Amount} each");
		}

		public void DisplayInsufficientCash(User user, Product product, uint count=1)
		{
			Console.WriteLine($"{user.Username} has insufficient cash to purchase {count}x {product.Name} for {product.Price} each");
		}

		public void DisplayGeneralError(string errorString)
		{
			Console.WriteLine(errorString);
		}

		private void ListProducts()
		{
			Console.WriteLine("Available products:");
			foreach (var product in _stregsystem.ActiveProducts)
			{
				Console.WriteLine($"{product.Id}: {product.Name} - {product.Price}");
			}
		}
		
		public void Start()
		{
			if (_running)
				throw new Exception("UI already running");
			ListProducts();
			_running = true;
			while (_running)
			{
				Console.Write("\nquickbuy> ");
				var input = Console.ReadLine()!;
				CommandEntered?.Invoke(input);;
			}
		}
		
		public void Close()
		{
			_running = false;
		}

		public event StregsystemEvent? CommandEntered;
	}
}