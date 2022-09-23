namespace Thismaker.Pluto
{
    /// <summary>
    /// A transaction is an individual process that occurs, and will involve the change
    /// of money from one hand to the next.
    /// </summary>
    public class PlutoTransaction : PlutoCharge, IPlutoTransaction
    {
        #region Props
        private TransactionType _type;

        /// <summary>
        /// The type of the transaction, e.g whether debit or credit
        /// </summary>
        public virtual TransactionType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        #endregion

        ///<inheritdoc/>
        public override void Update(IPlutoCharge source)
        {
            if (source.GetType().IsAssignableFrom(typeof(PlutoTransaction)))
            {
                PlutoTransaction derived = (PlutoTransaction)source;
                Type = derived.Type;
            }
            base.Update(source);
        }
    }
}
