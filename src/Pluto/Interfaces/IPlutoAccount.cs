using System.Collections.Generic;

namespace Thismaker.Pluto
{
    public interface IPlutoAccount
    {
        string Id { get; set; }
        string Name { get; set; }
        string Tag { get; set; }
        ICollection<IPlutoTransaction> Transactions { get; set; }
        AccountType Type { get; set; }
    }
}