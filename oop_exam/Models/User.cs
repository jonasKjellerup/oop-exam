using System;
using oop_exam.Csv;
using oop_exam.Util;

namespace oop_exam.Models
{
	public delegate void UserBalanceNotification(User user, decimal balance);
	
	public class User : IComparable<User>, IEquatable<User>
	{

		public event UserBalanceNotification? OnBalanceNotification;

		[CsvField("id")]
		public uint Id { get; }

		private string _firstname;

		[CsvField("firstname")]
		public string Firstname
		{
			get => _firstname;
			set
			{
				ValidationHelper.NullCheck(value, nameof(Firstname));
				_firstname = value;
			}
		}

		private string _lastname;

		[CsvField("lastname")]
		public string Lastname
		{
			get => _lastname;
			set
			{
				ValidationHelper.NullCheck(value, nameof(Lastname));
				_lastname = value;
			}
		}

		private string _username;

		[CsvField("username")]
		public string Username
		{
			get => _username;
			set
			{
				ValidationHelper.NullCheck(value, nameof(Username));
				ValidationHelper.CheckUsername(value);
				_username = value;
			}
		}

		private string _email;

		[CsvField("email")]
		public string Email
		{
			get => _email;
			set
			{
				ValidationHelper.NullCheck(value, nameof(Email));
				_email = ValidationHelper.ValidateEmail(value);
			}
		}

		private decimal _balance;
		
		[CsvField("balance")]
		public decimal Balance
		{
			get => _balance;
			set
			{
				if (value < 50)
					OnBalanceNotification?.Invoke(this, value);
				_balance = value;
			}
		}

		public User()
		{
			// TODO skal være fornuftig
		}

		public override bool Equals(object? obj) => obj switch
		{
			User other => other.Id == Id,
			_ => false
		};
		
		public bool Equals(User? other) => other?.Id == Id;

		public override string ToString() => $"{Firstname} {Lastname} ({Email})";

		public override int GetHashCode() => Id.GetHashCode();

		public int CompareTo(User? other) => Id.CompareTo(other?.Id);

	}
}