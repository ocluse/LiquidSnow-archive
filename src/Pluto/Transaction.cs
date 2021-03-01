using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Thismaker.Horus;

namespace Thismaker.Pluto
{
    public class Transaction : Charge
    {

        #region Props
        private TransactionType _type;

        public TransactionType Type 
        {
            get { return _type; }
            set { SetProperty(ref _type, value); } 
        }
        
        #endregion
        
        public Transaction():base()
        {

        }

        public Transaction(Transaction copy)
        {
            Date = copy.Date;
            Amount = copy.Amount;
            Details = copy.Details;
            Type = copy.Type;
            ID = copy.ID;
            Tag = copy.Tag;
        }

        public new static Transaction FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Transaction>(json);
        }

        public void Update(Transaction copy)
        {
            base.Update(copy);
            Type = copy.Type;
        }

    }

    public enum TransactionType { Debit, Credit }
}
