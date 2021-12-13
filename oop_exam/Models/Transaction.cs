using System;
using oop_exam.Util;

namespace oop_exam.Models
{
	public abstract class Transaction
	{
		public uint Id { get; set; }
		public User User { get; }
		public DateTime Date { get; }
		public decimal Amount { get; }

		public Transaction(User user, decimal amount)
		{
			ValidationHelper.NullCheck(user, nameof(user));
			User = user;
			Date = DateTime.Now;
			Amount = amount;
		}

		public abstract void Execute();
		
		public override string ToString() => $"[{Date}][ID={User.Id}] Transaction amount: {Amount} på bruger {User.Username}";
	}
}