using System.Collections.Generic;

namespace Thismaker.Pluto
{
    /// <summary>
    /// A book containing various accounts of a business
    /// </summary>
    public interface IPlutoBook
    {
        /// <summary>
        /// A collection of accounts
        /// </summary>
        ICollection<IPlutoAccount> Accounts { get; set; }
        /// <summary>
        /// The account ID to enter transactions into by default
        /// </summary>
        string DefaultAccountId { get; set; }
        /// <summary>
        /// The name of the book, can be the business name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Invoices billed to the business
        /// </summary>
        ICollection<IPlutoInvoice> ReceivedInvoices { get; set; }
        /// <summary>
        /// Invoices billed to the customers of the business
        /// </summary>
        ICollection<IPlutoInvoice> SentInvoices { get; set; }
        /// <summary>
        /// Add an invoice billed to the customers
        /// </summary>
        void AddReceivedInvoice(IPlutoInvoice invoice);
        /// <summary>
        /// Add a transaction to the specified account.
        /// </summary>
        void AddTransaction(string accountId, IPlutoTransaction transaction);
        /// <summary>
        /// Creates and adds an account to the book
        /// </summary>
        IPlutoAccount CreateAccount(string name, AccountType type);
        /// <summary>
        /// Creates and adds an account to the book
        /// </summary>
        IPlutoAccount CreateAccount(string name, string id, AccountType type);
        /// <summary>
        /// Creates and adds an invoice that is to be billed to customers
        /// </summary>
        IPlutoInvoice CreateSentInvoice();
        /// <summary>
        /// Creates an invoice to be sent from the provided source
        /// </summary>
        IPlutoInvoice CreateSentInvoice(IPlutoInvoice source);
        /// <summary>
        /// Deletes a transaction and returns true if the deletion was successful.
        /// </summary>
        bool DeleteTransaction(string accountId, string transactionId);
        /// <summary>
        /// Updates a transaction in the provided account with the provided values.
        /// </summary>
        void EditTransaction(string accountId, IPlutoTransaction transaction);
        /// <summary>
        /// Returns an account with the specified ID 
        /// </summary>
        IPlutoAccount GetAccount(string accountId);
    }
}