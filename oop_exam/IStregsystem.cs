using System;
using System.Collections.Generic;
using oop_exam.Models;

namespace oop_exam
{
	public interface IStregsystem
	{
		IEnumerable<Product> ActiveProducts { get; }
		InsertCashTransaction AddCreditsToAccount(User user, decimal amount);
		BuyTransaction BuyProduct(User user, Product product);
		Product GetProductById(uint id);
		IEnumerable<Transaction> GetTransactions(User user, int count);
		IEnumerable<User> GetUsers(Func<User, bool> predicate);
		User GetUserByUsername(string username);
		event UserBalanceNotification UserBalanceWarning;
	}
}