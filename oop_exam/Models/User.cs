using System;
using oop_exam.Csv;
using oop_exam.Util;

namespace oop_exam.Models
{
    public delegate void UserBalanceNotification(User user);

    public class User : IComparable<User>, IEquatable<User>
    {
        public UserBalanceNotification? OnBalanceNotification;

        [CsvField("id")]
        public uint Id { get; init; }

        private string _firstname = null!;

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

        private string _lastname = null!;

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

        private string _username = null!;

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

        private string _email = null!;

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
                _balance = value;
                if (value < 50)
                    OnBalanceNotification?.Invoke(this);
            }
        }

        public User(string firstname, string lastname, string username, string email, decimal balance = 0)
        {
            Firstname = firstname;
            Lastname = lastname;
            Username = username;
            Email = email;
            Balance = balance;
        }

        public User(IdDistributor distributor, string firstname, string lastname, string username, string email,
            decimal balance = 0) : this(firstname, lastname, username, email, balance)
        {
            Id = distributor.NextId<User>();
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