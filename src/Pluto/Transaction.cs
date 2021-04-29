namespace Thismaker.Pluto
{
    public class Transaction : Charge
    {

        #region Props
        private TransactionType _type;

        public virtual TransactionType Type 
        {
            get { return _type; }
            set { SetProperty(ref _type, value); } 
        }
        
        #endregion
        
        public Transaction():base()
        {

        }

        public virtual void Update(Transaction copy)
        {
            base.Update(copy);
            Type = copy.Type;
        }

    }

    public enum TransactionType { Debit, Credit }
}
