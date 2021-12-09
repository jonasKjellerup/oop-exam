namespace oop_exam.Models
{
	public class InsertCashTransaction : Transaction
	{
		public InsertCashTransaction(User user, decimal amount) : base(user, amount)
		{
		}

		public override string ToString() => $"[{Date}][ID={User.Id}] Indbetaling af {Amount} til {User.Username}";

		public override void Execute()
		{
			User.Balance += Amount;
		}
	}
}