using System;
using System.Collections.ObjectModel;
using System.Linq;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    public class Book : BindableBase
    {
        #region Private Fields
        private int _invoiceIndexer;
        private string _defaultID, _name;
        private ObservableCollection<Account> _accounts;
        private ObservableCollection<Invoice> _sentInvoices;
        private ObservableCollection<Invoice> _receivedInvoices;
        #endregion

        #region Initialization
        /// <summary>
        /// Creates a new <see cref="Book"/> with the provided name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="addCashAccount">If true, an account called 'Cash Account' is added to 
        /// the book and made the <see cref="DefaultCashAccount"/></param>
        public Book(string name, bool addCashAccount = true)
        {
            Name = name;

            Accounts = new ObservableCollection<Account>();
            SentInvoices = new ObservableCollection<Invoice>();
            ReceivedInvoices = new ObservableCollection<Invoice>();

            if (addCashAccount)
            {
                var acc = new Account
                {
                    Name = "Cash Account"
                };
                DefaultID = acc.ID;
                Accounts.Add(acc);
            }

        }

        /// <summary>
        /// Creates a new empty <see cref="Book"/>
        /// </summary>
        public Book()
        {
            Accounts = new ObservableCollection<Account>();
            SentInvoices = new ObservableCollection<Invoice>();
            ReceivedInvoices = new ObservableCollection<Invoice>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The ID of the default cash account.
        /// </summary>
        public virtual string DefaultID
        {
            get => _defaultID;
            set => SetProperty(ref _defaultID, value);
        }

        /// <summary>
        /// The default cash account defined by the <see cref="DefaultID"/>
        /// If there's no <see cref="DefaultID"/> then the method returns null.
        /// </summary>
        public Account DefaultCashAccount
        {
            get
            {
                return Accounts?.First(x => x.ID == DefaultID);
            }
        }

        /// <summary>
        /// The name of the book of account. Usually the name of the firm that owns the book
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        /// <summary>
        /// The list of accounts/ledgers for the firm. Accounts usually hold everything
        /// </summary>
        public virtual ObservableCollection<Account> Accounts
        {
            get => _accounts;
            set => SetProperty(ref _accounts, value);
        }

        /// <summary>
        /// Invoices that have been sent to debtors/customers
        /// </summary>
        public virtual ObservableCollection<Invoice> SentInvoices
        {
            get => _sentInvoices;
            set => SetProperty(ref _sentInvoices, value);
        }

        /// <summary>
        /// Invoices received from creditors/suppliers
        /// </summary>
        public virtual ObservableCollection<Invoice> ReceivedInvoices
        {
            get => _receivedInvoices;
            set => SetProperty(ref _receivedInvoices, value);
        }

        public virtual int InvoiceIndexer
        {
            get => _invoiceIndexer;
            set => SetProperty(ref _invoiceIndexer, value);
                
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Attempts to create a new account, returning the provided account
        /// </summary>
        /// <param name="name">The name of the account</param>
        /// <param name="type">The type of the account</param>
        /// <returns>A newly created account</returns>
        public Account CreateAccount(string name, AccountType type)
        {
            var acc = new Account
            {
                Name = name,
                Type = type
            };
            Accounts.Add(acc);

            return acc;
        }

        /// <summary>
        /// Attempts to create a new account, returning the new account if successful.
        /// </summary>
        /// <param name="name">The name of the account</param>
        /// <param name="id">The id to give the account</param>
        /// <param name="type">The type of the account</param>
        /// <returns>A newly created account</returns>
        /// <exception cref="InvalidOperationException">When the account already exists</exception>
        public Account CreateAccount(string name, string id, AccountType type)
        {
            var acc = GetAccount(id);
            if (acc != null)
            {
                throw new InvalidOperationException("An account with that id already exists");
            }

            acc = new Account
            {
                Name = name,
                ID = id,
                Type = type
            };

            Accounts.Add(acc);
            return acc;
        }

        /// <summary>
        /// Finds the account with the provided ID.
        /// </summary>
        /// <param name="accountId">The id of the account to find</param>
        /// <returns>Null if no account was found</returns>
        public Account GetAccount(string accountId)
        {
            return Accounts.FirstOrDefault(x => x.ID == accountId);
        }

        /// <summary>
        /// The recommended way to change the default account. As it checks if the new account exists
        /// </summary>
        /// <param name="newId">The ID of the new account</param>
        /// <exception cref="InvalidOperationException">When the ID does not point to an existing account</exception>
        public void ChangeDefaultAccount(string newId)
        {
            if (newId == DefaultID) return;
            var acc = GetAccount(newId);
            if (acc == null)
            {
                throw new InvalidOperationException("No account found with the provided ID");
            }
            DefaultID = acc.ID;
        }
        
        /// <summary>
        /// Adds a transaction to the account identified by the provided id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="transaction"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddTransaction(string accountId, Transaction transaction)
        {
            var acc = Accounts.FirstOrDefault(x => x.ID == accountId);
            if (acc == null) throw new InvalidOperationException("No account found with the provided ID");

            acc.Transactions.Add(transaction);
        }

        /// <summary>
        /// Edits a transaction in an account identified by the provided id
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="transaction"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void EditTransaction(string accountId, Transaction transaction)
        {
            var acc = Accounts.FirstOrDefault(x => x.ID == accountId);
            if (acc == null) throw new InvalidOperationException("No account found with the provided ID");

            var trans = acc.Transactions.FirstOrDefault(x => x.ID == transaction.ID);
            if (trans == null)
            {
                throw new InvalidOperationException("Provided transaction does not exist in the account");
            }

            trans.Update(transaction);
        }

        /// <summary>
        /// Attempts to delete the provided transaction from the account
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="transaction"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void DeleteTransaction(string accountId, string transactionId)
        {
            var acc = Accounts.FirstOrDefault(x => x.ID == accountId);
            if (acc == null) throw new InvalidOperationException("No account found with the provided ID");

            var trans = acc.Transactions.FirstOrDefault(x => x.ID == transactionId);
            if (trans == null)
            {
                throw new InvalidOperationException("Provided transaction does not exist in the account");
            }

            acc.Transactions.Remove(trans);
        }

        /// <summary>
        /// Creates a new blank invoice that is automatically indexed
        /// </summary>
        /// <returns>The created invoice</returns>
        public Invoice CreateSentInvoice()
        {
            InvoiceIndexer++;
            var invoice= new Invoice(InvoiceIndexer);
            SentInvoices.Add(invoice);
            return invoice;
        }

        /// <summary>
        /// Creates an invoice from the provided invoice then indexes it
        /// </summary>
        /// <param name="source">The invoice to get the data from</param>
        /// <returns>The created invoice</returns>
        public Invoice CreateSentInvoice(Invoice source)
        {

            var invoice = new Invoice();
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
        public void AddReceivedInvoice(Invoice invoice)
        {
            ReceivedInvoices.Add(invoice);
        }
        #endregion
    }
}
