using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Thismaker.Pluto
{
    public class Book
    {
        public string DefaultID { get; set; }
        
        [JsonIgnore]
        public Account DefaultCashAccount
        {
            get
            {
                return Accounts.First(x => x.AccountID == DefaultID);
            }
        }
        public string Name { get; set; }

        public ObservableCollection<Account> Accounts;

        public Book(string name)
        {
            Name = name;
            var acc = new Account
            {
                Name = "Cash Account"
            };
            DefaultID = acc.AccountID;
            Accounts = new ObservableCollection<Account>
            {
                acc
            };
        }

        public Account FindAccount(string id)
        {
            return Accounts.FirstOrDefault(x => x.AccountID == id);
        }

        public static Book FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Book>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void CreateAccount(string name, AccountType type)
        {
            var acc = new Account
            {
                Name = name,
                Type = type
            };
            Accounts.Add(acc);

        }

        public void CreateAccount(string name, string id, AccountType type)
        {
            var acc = GetAccount(id);
            if (acc != null)
            {
                throw new InvalidOperationException("An account with that id already exists");
            }

            acc = new Account
            {
                Name = name,
                AccountID = id,
                Type = type
            };

            Accounts.Add(acc);
        }

        public Account GetAccount(string accountId)
        {
            return Accounts.FirstOrDefault(x => x.AccountID == accountId);
        }

        public void ChangeDefaultAccount(string newId)
        {
            if (newId == DefaultID) return;
            var acc = GetAccount(newId);
            if (acc == null)
            {
                throw new InvalidOperationException("No account found with the provided ID");
            }
            DefaultID = acc.AccountID;
        }
        
        public void AddTransaction(string accountId, Transaction transaction)
        {
            var acc = Accounts.FirstOrDefault(x => x.AccountID == accountId);
            if (acc == null) throw new InvalidOperationException("No account found with the provided ID");

            acc.Transactions.Add(transaction);
        }

        public void EditTransaction(string accountId, Transaction transaction)
        {
            var acc = Accounts.FirstOrDefault(x => x.AccountID == accountId);
            if (acc == null) throw new InvalidOperationException("No account found with the provided ID");

            var trans = acc.Transactions.FirstOrDefault(x => x.ID == transaction.ID);
            if (trans == null)
            {
                throw new InvalidOperationException("Provided transaction does not exist in the account");
            }

            trans.Update(transaction);
        }

        public void DeleteTransaction(string accountId, Transaction transaction)
        {
            var acc = Accounts.FirstOrDefault(x => x.AccountID == accountId);
            if (acc == null) throw new InvalidOperationException("No account found with the provided ID");

            var trans = acc.Transactions.FirstOrDefault(x => x.ID == transaction.ID);
            if (trans == null)
            {
                throw new InvalidOperationException("Provided transaction does not exist in the account");
            }

            acc.Transactions.Remove(trans);
        }
    }
}
