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

		public void DisplayProductNotFound(string product)
		{
			Console.WriteLine($"Product by name: {product} not found.");
		}

		public void DisplayUserInfo(User user)
		{
			throw new System.NotImplementedException();
		}

		public void DisplayTooManyArgumentsError(string command)
		{
			throw new System.NotImplementedException();
		}

		public void DisplayAdminCommandNotFoundMessage(string adminCommand)
		{
			throw new System.NotImplementedException();
		}

		public void DisplayUserBuysProduct(BuyTransaction transaction)
		{
			throw new System.NotImplementedException();
		}

		public void DisplayUserBuysProduct(int count, BuyTransaction transaction)
		{
			throw new System.NotImplementedException();
		}

		public void DisplayInsufficientCash(User user, Product product)
		{
			throw new System.NotImplementedException();
		}

		public void DisplayGeneralError(string errorString)
		{
			throw new System.NotImplementedException();
		}

		public void Start()
		{
			if (_running)
				throw new Exception("UI already running"); // TODO create custom exception
			_running = true;
			while (_running)
			{
				
			}
			throw new System.NotImplementedException();
		}
		
		public void Close()
		{
			_running = false;
		}

		public event StregsystemEvent? CommandEntered;
	}
}