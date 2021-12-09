using System;
using System.Collections.Generic;
using System.Linq;
using oop_exam.Exceptions;
using oop_exam.Models;

namespace oop_exam
{
	public class Stregsystem : IStregsystem
	{
		public event UserBalanceNotification? UserBalanceWarning;

		private List<User> _users = new List<User>();
		private Dictionary<uint, Product> _products = new Dictionary<uint, Product>();
		private List<Transaction> _transactions = new List<Transaction>();

		public BuyTransaction BuyProduct(User user, Product product) => new BuyTransaction(product, user);

		public InsertCashTransaction AddCreditsToAccount(User user, decimal amount) =>
			new InsertCashTransaction(user, amount);

		public void ExecuteTransaction(Transaction transaction)
		{
			transaction.Execute();
			_transactions.Add(transaction);
		}

		public Product GetProductById(uint id)
		{
			if (_products.ContainsKey(id))
				return _products[id];

			throw new UnknownProductIdException(id);
		}

		public IEnumerable<User> GetUsers(Func<User, bool> predicate) => _users.Where(predicate);

		public User GetUserByUsername(string username)
		{
			var user = _users.Find(user => user.Username == username);
			if (user == null)
				throw new UnknownUsernameException(username);
			return user;
		}

		public IEnumerable<Transaction> GetTransactions(User user, int count) =>
			_transactions
				.Where(t => Equals(t.User, user))
				.OrderByDescending(t => t.Date)
				.Take(count);

		public IEnumerable<Product> ActiveProducts => _products.Values.Where(p => p.Active);
	}
}