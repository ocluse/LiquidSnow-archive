using System;
using System.Collections.Generic;

namespace Thismaker.Pluto
{
    public interface IPlutoInvoice
    {
        decimal Balance { get; }
        string BilledTo { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateDue { get; set; }
        string Details { get; set; }
        decimal Discount { get; set; }
        decimal DiscountedTotal { get; }
        string Id { get; set; }
        int Index { get; set; }
        ICollection<IPlutoInvoiceItem> Items { get; set; }
        decimal Paid { get; set; }
        string Sender { get; set; }
        decimal Shipping { get; set; }
        decimal SubTotal { get; }
        string Tag { get; set; }
        decimal Tax { get; set; }
        decimal Total { get; }

        IPlutoTransaction GetTransaction();
        IPlutoTransaction GetTransaction(string details);
        IPlutoTransaction GetTransaction(string details, TransactionType type);
        IPlutoTransaction GetTransaction(TransactionType type);
    }
}