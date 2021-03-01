using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Thismaker.Core;
using Thismaker.Horus;

namespace Thismaker.Pluto
{
    public class Account : BindableBase
    {
        #region Props
        private string _name, _id;
        private AccountType _type;
        private ObservableCollection<Transaction> _transactions;

        public string AccountID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string Name 
        {
            get { return _name; }
            set { SetProperty(ref _name, value, nameof(Name)); }
        }
        
        public ObservableCollection<Transaction> Transactions
        {
            get { return _transactions; }
            set { SetProperty(ref _transactions, value, nameof(Transactions)); }
        }

        public AccountType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value, nameof(Type)); }
        }
        #endregion

        public Account()
        {
            AccountID = Horus.Horus.GenerateID();
            Transactions = new ObservableCollection<Transaction>();
        }

        public Account(Account copy)
        {
            AccountID = copy.AccountID;
            Name = copy.Name;
            Type = copy.Type;
            Transactions = new ObservableCollection<Transaction>(copy.Transactions);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Account FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Account>(json);
        }

        public void GenerateNewID()
        {
            AccountID = Horus.Horus.GenerateID();
        }
    }

    public enum AccountType
    {
        Asset, Liability, Equity, Payable, Receiveable, Income, Expense, Cash, Bank
    }

}
