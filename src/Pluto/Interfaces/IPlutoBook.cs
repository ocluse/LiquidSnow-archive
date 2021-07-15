using System.Collections.Generic;

namespace Thismaker.Pluto
{
    public interface IPlutoBook
    {
        ICollection<IPlutoAccount> Accounts { get; set; }
        string DefaultAccountId { get; set; }
        string Name { get; set; }
        ICollection<IPlutoInvoice> ReceivedInvoices { get; set; }
        ICollection<IPlutoInvoice> SentInvoices { get; set; }

        void AddReceivedInvoice(IPlutoInvoice invoice);
        void AddTransaction(string accountId, IPlutoTransaction transaction);
        IPlutoAccount CreateAccount(string name, AccountType type);
        IPlutoAccount CreateAccount(string name, string id, AccountType type);
        IPlutoInvoice CreateSentInvoice();
        IPlutoInvoice CreateSentInvoice(IPlutoInvoice source);
        bool DeleteTransaction(string accountId, string transactionId);
        void EditTransaction(string accountId, IPlutoTransaction transaction);
        IPlutoAccount GetAccount(string accountId);
    }
}