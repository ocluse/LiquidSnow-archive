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

        public virtual string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public virtual string Name 
        {
            get { return _name; }
            set { SetProperty(ref _name, value, nameof(Name)); }
        }
        
        public virtual ObservableCollection<Transaction> Transactions
        {
            get { return _transactions; }
            set { SetProperty(ref _transactions, value, nameof(Transactions)); }
        }

        public virtual AccountType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value, nameof(Type)); }
        }
        #endregion

        public Account()
        {
            ID = Horus.Horus.GenerateID();
            Transactions = new ObservableCollection<Transaction>();
        }
    }

    public enum AccountType
    {
        Asset, Liability, Equity, Payable, Receiveable, Income, Expense, Cash, Bank
    }

}
