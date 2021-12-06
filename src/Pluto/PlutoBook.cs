using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    /// <summary>
    /// A book is a collection of several accounts and invoices.
    /// It usually represents totally the financial makeup of an entity
    /// </summary>
    public class PlutoBook : BindableBase, IPlutoBook
    {
        #region Private Fields
        private int _invoiceIndexer;
        private string _defaultId, _name;
        private ICollection<IPlutoAccount> _accounts;
        private ICollection<IPlutoInvoice> _sentInvoices;
        private ICollection<IPlutoInvoice> _receivedInvoices;
        #endregion

        #region Properties
        /// <summary>
        /// The unique identifer of the default account.
        /// Usually the Id of the cash account.
        /// </summary>
        public virtual string DefaultAccountId
        {
            get => _defaultId;
            set => SetProperty(ref _defaultId, value);
        }

        /// <summary>
        /// The default account defined by the <see cref="DefaultAccountId"/>
        /// If there's no <see cref="DefaultAccountId"/> then the method returns null.
        /// </summary>
        public IPlutoAccount DefaultAccount => Accounts?.FirstOrDefault(x => x.Id == DefaultAccountId);

        /// <summary>
        /// The name of the book of account. Usually the name of the firm that owns the book
        /// </summary>
        public virtual string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        /// <summary>
        /// A collection of the firms accounts that hold all the transactions
        /// </summary>
        public virtual ICollection<IPlutoAccount> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        /// <summary>
        /// Invoices that have been sent to debtors/customers
        /// </summary>
        public virtual ICollection<IPlutoInvoice> SentInvoices
        {
            get => _sentInvoices;
            set => SetProperty(ref _sentInvoices, value);
        }

        /// <summary>
        /// Invoices received from creditors/suppliers
        /// </summary>
        public virtual ICollection<IPlutoInvoice> ReceivedInvoices
        {
            get => _receivedInvoices;
            set => SetProperty(ref _receivedInvoices, value);
        }

        /// <summary>
        /// When used, invoices are automatically indexed depending on
        /// the value of the current indexer
        /// </summary>
        public virtual int InvoiceIndexer
        {
            get => _invoiceIndexer;
            set => SetProperty(ref _invoiceIndexer, value);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a new account with the provided details and adds it to the list of accounts
        /// </summary>
        /// <param name="name">The name of the account</param>
        /// <param name="type">The type of the account</param>
        /// <returns>A newly created account with a unique Id</returns>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Accounts"/> is null</exception>
        public IPlutoAccount CreateAccount(string name, AccountType type)
        {
            if (Accounts is null)
            {
                throw new InvalidOperationException($"{nameof(Accounts)} was null");
            }

            var account = new PlutoAccount
            {
                Name = name,
                Type = type,
                Id = Horus.Horus.GenerateId()
            };

            Accounts.Add(account);

            return account;
        }

        /// <summary>
        /// Creates a new account with the provided details, adding it to the accounts list
        /// </summary>
        /// <param name="name">The name of the account</param>
        /// <param name="id">The id to give the account</param>
        /// <param name="type">The type of the account</param>
        /// <returns>A newly created account with the provided Id</returns>
        /// <exception cref="InvalidOperationException">When <see cref="Accounts"/> is null 
        /// or when an account with a similar Id already exists</exception>
        public IPlutoAccount CreateAccount(string name, string id, AccountType type)
        {
            if (Accounts is null)
            {
                throw new InvalidOperationException($"{nameof(Accounts)} was null");
            }

            var account = GetAccount(id);
            if (account != null)
            {
                throw new InvalidOperationException("An account with that Id already exists");
            }

            account = new PlutoAccount
            {
                Name = name,
                Id = id,
                Type = type
            };


            Accounts.Add(account);
            return account;
        }

        /// <summary>
        /// Returns the account with the provided Id.
        /// </summary>
        /// <param name="accountId">The id of the account to find</param>
        /// <returns>Null if no account was found, otherwise the account itself</returns>
        public IPlutoAccount GetAccount(string accountId)
        {
            return Accounts?.FirstOrDefault(x => x.Id == accountId);
        }

        /// <summary>
        /// The recommended way to change the default account. As it checks if the new default account exists
        /// </summary>
        /// <param name="newId">The ID of the new account</param>
        ///<exception cref="AccountNotFoundException"></exception>
        public void ChangeDefaultAccount(string newId)
        {
            if (newId == DefaultAccountId) return;
            var acc = GetAccount(newId);
            if (acc == null)
            {
                throw new AccountNotFoundException();
            }
            DefaultAccountId = acc.Id;
        }

        /// <summary>
        /// Adds a transaction to the account identified by the provided id
        /// </summary>
        /// <exception cref="AccountNotFoundException">When the account does not exist</exception>
        public void AddTransaction(string accountId, IPlutoTransaction transaction)
        {
            var acc = Accounts?.FirstOrDefault(x => x.Id == accountId);
            if (acc == null) throw new AccountNotFoundException();

            acc.Transactions.Add(transaction);
        }

        /// <summary>
        /// Edits a transaction in an account identified by the provided id
        /// </summary>
        /// <exception cref="AccountNotFoundException"></exception>
        /// <exception cref="TransactionNotFoundException"></exception>
        public void EditTransaction(string accountId, IPlutoTransaction source)
        {
            var acc = Accounts?.FirstOrDefault(x => x.Id == accountId);

            if (acc == null) throw new AccountNotFoundException();

            var trans = acc.Transactions.FirstOrDefault(x => x.Id == source.Id);
            if (trans == null)
            {
                throw new TransactionNotFoundException();
            }

            trans.Amount = source.Amount;
            trans.Date = source.Date;
            trans.Details = source.Details;
            trans.Id = source.Id;
            trans.Tag = source.Tag;
            trans.Type = source.Type;
        }

        /// <summary>
        /// Attempts to delete the provided transaction from the account
        /// </summary>
        /// <returns>True if that transaction has been removed successfully</returns>
        /// <param name="accountId"></param>
        /// <param name="transaction"></param>
        /// <exception cref="AccountNotFoundException"></exception>
        /// <exception cref="TransactionNotFoundException"></exception>
        public bool DeleteTransaction(string accountId, string transactionId)
        {
            var acc = Accounts?.FirstOrDefault(x => x.Id == accountId);
            if (acc == null) throw new AccountNotFoundException();

            var trans = acc.Transactions.FirstOrDefault(x => x.Id == transactionId);
            if (trans == null)
            {
                throw new TransactionNotFoundException();
            }

            return acc.Transactions.Remove(trans);
        }

        /// <summary>
        /// Creates a new blank invoice that is automatically indexed
        /// </summary>
        /// <returns>The created invoice</returns>
        public IPlutoInvoice CreateSentInvoice()
        {
            InvoiceIndexer++;
            var invoice = new PlutoInvoice
            {
                Index = InvoiceIndexer
            };

            SentInvoices.Add(invoice);
            return invoice;
        }

        /// <summary>
        /// Creates an invoice from the provided invoice then indexes it
        /// </summary>
        /// <param name="source">The invoice to get the data from</param>
        /// <returns>The created invoice</returns>
        public IPlutoInvoice CreateSentInvoice(IPlutoInvoice source)
        {

            var invoice = new PlutoInvoice();
            invoice.Update(source);
            InvoiceIndexer++;
            invoice.Index = InvoiceIndexer;
            SentInvoices.Add(invoice);
            return invoice;
        }

        /// <summary>
        /// Adds the invoice as is to the received stack
        /// </summary>
        /// <param name="invoice"></param>
        public void AddReceivedInvoice(IPlutoInvoice invoice)
        {
            ReceivedInvoices.Add(invoice);
        }
        #endregion
    }
}
