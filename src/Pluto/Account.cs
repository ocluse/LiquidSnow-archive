using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using Thismaker.Enigma;

namespace Thismaker.Pluto
{
    public class Account:INotifyPropertyChanged
    {
        #region Props
        private string name;
        private AccountType type;
        private ObservableCollection<Transaction> transactions;

        public string AccountID { get; set; }

        public string Name 
        {
            get { return name; }
            set { SetFied(ref name, value, nameof(Name)); }
        }
        
        public ObservableCollection<Transaction> Transactions
        {
            get { return transactions; }
            set { SetFied(ref transactions, value, nameof(Transactions)); }
        }
        public AccountType Type
        {
            get { return type; }
            set { SetFied(ref type, value, nameof(Type)); }
        }
        #endregion

        #region INotifyImplementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetFied<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        public Account()
        {
            AccountID = Enigma.Enigma.GenerateID();
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
            AccountID = Enigma.Enigma.GenerateID();
        }
    }

    public enum AccountType
    {
        Asset, Liability, Equity, Payable, Receiveable, Income, Expense, Cash, Bank

    }

}
