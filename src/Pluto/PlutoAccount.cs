using System.Collections.Generic;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    /// <summary>
    /// An account represents a collection of transactions that have transpired with a provided entity.
    /// For example, transactions towards the payments of rent.
    /// </summary>
    public class PlutoAccount : BindableBase, IPlutoAccount
    {
        #region Private Fields
        private string _name, _id, _tag;
        private AccountType _type;
        private ICollection<IPlutoTransaction> _transactions;
        #endregion

        #region Properties

        /// <summary>
        /// The unchanging unique Id of the account
        /// </summary>
        public virtual string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// The Name of the Account e.g Cash Account
        /// </summary>
        public virtual string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, nameof(Name));
        }

        /// <summary>
        /// A tag that can be added to the account for more details
        /// </summary>
        public virtual string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }

        /// <summary>
        /// The collection of transactions performed on the account
        /// </summary>
        public virtual ICollection<IPlutoTransaction> Transactions
        {
            get => _transactions;
            set => SetProperty(ref _transactions, value, nameof(Transactions));
        }

        /// <summary>
        /// The type of the account, e.g whether it is an asset or credit
        /// </summary>
        public virtual AccountType Type
        {
            get => _type;
            set => SetProperty(ref _type, value, nameof(Type));
        }
        #endregion
    }
}
