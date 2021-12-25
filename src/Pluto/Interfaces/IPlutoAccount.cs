using System.Collections.Generic;

namespace Thismaker.Pluto
{
    /// <summary>
    /// The interface that represents a single book of account.
    /// </summary>
    public interface IPlutoAccount
    {
        string Id { get; set; }
        string Name { get; set; }
        string Tag { get; set; }
        ICollection<IPlutoTransaction> Transactions { get; set; }
        AccountType Type { get; set; }
    }
}