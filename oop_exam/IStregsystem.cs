using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using oop_exam.Models;

namespace oop_exam
{
	public interface IStregsystem
	{
		IEnumerable<Product> ActiveProducts { get; }
		void AddCreditsToAccount(User user, decimal amount);
		void BuyProduct(User user, Product product);

		void ExecuteTransaction(Transaction transaction);
		Product GetProductById(uint id);

		bool TryGetProductById(uint id, [MaybeNullWhen(false)]out Product product);
		
		IEnumerable<Transaction> GetTransactions(User user, int count);
		IEnumerable<User> GetUsers(Func<User, bool> predicate);
		User GetUserByUsername(string username);

		bool TryGetUserByUsername(string username, [MaybeNullWhen(false)]out User user);
		
		event UserBalanceNotification UserBalanceWarning;
	}
}