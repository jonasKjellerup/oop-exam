using System;
using System.Diagnostics;
using System.Globalization;
using oop_exam.Csv;
using oop_exam.Util;

namespace oop_exam.Models
{
	public abstract class Transaction
	{
		[CsvField("id")]
		public uint Id { get; set; }
		public User User { get; }
		public DateTime Date { get; }
		
		[CsvField("amount")]
		public decimal Amount { get; }

		public Transaction(User user, decimal amount)
		{
			ValidationHelper.NullCheck(user, nameof(user));
			User = user;
			Date = DateTime.Now;
			Amount = amount;
		}

		public Transaction Clone() => (Transaction) MemberwiseClone();

		public abstract void Execute();
		
		public override string ToString() => $"[{Date}][ID={User.Id}] Transaction amount: {Amount} på bruger {User.Username}";

		// The below region contains a series of fields
		// that are used when serializing transactions for logging.
		
		#region Logging fields

		[CsvField("user")]
		public uint UserId => User.Id;

		[CsvField("date")]
		public string DateString => Date.ToString(CultureInfo.InvariantCulture);

		[CsvField("type")]
		public string Type => this switch
		{
			BuyTransaction => "purchase",
			InsertCashTransaction => "insert",
			_ => "unknown",
		};

		[CsvField("product")]
		public string ProductField => this switch
		{
			BuyTransaction b => b.Product.Id.ToString(),
			_ => "",
		};

		#endregion
		
		
		
	}
}