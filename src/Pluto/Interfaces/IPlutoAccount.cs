using System.Collections.Generic;

namespace Thismaker.Pluto
{
    /// <summary>
    /// The interface that represents a single book of account.
    /// </summary>
    public interface IPlutoAccount
    {
        /// <summary>
        /// The unique ID
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// The name of the account
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The tag of the account, can contain additional information
        /// </summary>
        string Tag { get; set; }
        /// <summary>
        /// A collection of transactions comprising the account
        /// </summary>
        ICollection<IPlutoTransaction> Transactions { get; set; }
        /// <summary>
        /// The type of the account
        /// </summary>
        AccountType Type { get; set; }
    }
}