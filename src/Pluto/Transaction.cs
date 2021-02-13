using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Thismaker.Horus;

namespace Thismaker.Pluto
{
    public class Transaction : INotifyPropertyChanged
    {

        #region Props

        private DateTime date;
        private decimal amount;
        private string details;
        private TransactionType type;

        public string ID { get; set; }

        public DateTime Date 
        { 
            get { return date; } 
            set { SetFied(ref date, value, nameof(Date)); } 
        }
        public decimal Amount 
        {
            get { return amount; }
            set { SetFied(ref amount, value, nameof(Amount)); }
        }
        public string Details 
        {
            get { return details; }
            set { SetFied(ref details, value, nameof(Details)); }
        }
        public TransactionType Type 
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
        
        public Transaction()
        {
            ID = Horus.Horus.GenerateID(IDKind.Hash);
        }

        public Transaction(Transaction copy)
        {
            Date = copy.Date;
            Amount = copy.Amount;
            Details = copy.Details;
            Type = copy.Type;
            ID = copy.ID;
        }

        public static Transaction FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Transaction>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void Update(Transaction copy)
        {
            Date = copy.Date;
            Amount = copy.Amount;
            Details = copy.Details;
            Type = copy.Type;
        }

    }

    public enum TransactionType { Debit, Credit }
}
