namespace Thismaker.Pluto
{
    /// <summary>
    /// A transaction added to an account.
    /// </summary>
    public interface IPlutoTransaction : IPlutoCharge
    {
        /// <summary>
        /// The type of the transaction
        /// </summary>
        TransactionType Type { get; set; }
    }
}