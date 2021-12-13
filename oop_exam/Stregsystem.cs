using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using oop_exam.Csv;
using oop_exam.Exceptions;
using oop_exam.Models;
using oop_exam.Util;

namespace oop_exam
{
    public class Stregsystem : IStregsystem
    {
        public event UserBalanceNotification? UserBalanceWarning;

        private List<User> _users; // TODO maintain type consistency between user and product collections
        private Dictionary<uint, Product> _products;
        private List<Transaction> _transactions = new();
        private readonly IdDistributor _idDistributor = new();

        public Stregsystem()
        {
            // Load data from files
            var serializer = new CsvSerializer();
            using (var userFile = File.OpenRead("Data/users.csv"))
            {
                _users = serializer.Deserialize<User>(userFile, ',');
                uint maxId = 0;
                foreach (var user in _users)
                {
                    if (user.Id > maxId)
                        maxId = user.Id;

                    user.OnBalanceNotification = OnUserBalanceNotification;
                }
                
                _idDistributor.Notify<User>(_users.Max()!.Id);
            }

            using (var productFile = File.OpenRead("Data/products.csv"))
            {
                var products = serializer.Deserialize<Product>(productFile, ';');
                _idDistributor.Notify<Product>(products.Max(p => p.Id));
                // Convert list to dictionary mapping id to product
                _products = new Dictionary<uint, Product>(
                    products.Select(p => new KeyValuePair<uint, Product>(p.Id, p))
                );
            }
        }

        private void OnUserBalanceNotification(User user)
        {
            UserBalanceWarning?.Invoke(user);
        }

        public void BuyProduct(User user, Product product)
        {
            ExecuteTransaction(new BuyTransaction(product, user));
        }

        public void AddCreditsToAccount(User user, decimal amount)
        {
            ExecuteTransaction(new InsertCashTransaction(user, amount));
        }

        public void ExecuteTransaction(Transaction transaction)
        {
            if (transaction.Id == 0)
                transaction.Id = _idDistributor.NextId<Transaction>();
            
            transaction.Execute();
            _transactions.Add(transaction);
        }

        public Product GetProductById(uint id)
        {
            if (_products.ContainsKey(id))
                return _products[id];

            throw new UnknownProductIdException(id);
        }

        public bool TryGetProductById(uint id, [MaybeNullWhen(false)] out Product product)
        {
            try
            {
                product = GetProductById(id);
                return true;
            }
            catch (Exception)
            {
                product = null;
                return false;
            }
        }

        public IEnumerable<User> GetUsers(Func<User, bool> predicate) => _users.Where(predicate);

        public User GetUserByUsername(string username)
        {
            var user = _users.Find(user => user.Username == username);
            if (user == null)
                throw new UnknownUsernameException(username);
            return user;
        }

        public bool TryGetUserByUsername(string username, [MaybeNullWhen(false)] out User user)
        {
            try
            {
                user = GetUserByUsername(username);
                return true;
            }
            catch (Exception)
            {
                user = null;
                return false;
            }
        }

        public IEnumerable<Transaction> GetTransactions(User user, int count) =>
            _transactions
                .Where(t => Equals(t.User, user))
                .OrderByDescending(t => t.Date)
                .Take(count);

        public IEnumerable<Product> ActiveProducts => _products.Values.Where(p => p.Active);
    }
}