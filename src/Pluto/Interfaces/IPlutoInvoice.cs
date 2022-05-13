using System;
using System.Collections.Generic;

namespace Thismaker.Pluto
{
    /// <summary>
    /// Defines the framework of an invoice
    /// </summary>
    public interface IPlutoInvoice
    {
        /// <summary>
        /// The invoice balance, i.e. how much is still left.
        /// </summary>
        decimal Balance { get; }
        /// <summary>
        /// The party the invoice is billed to and is supposed to pay.
        /// </summary>
        string BilledTo { get; set; }
        /// <summary>
        /// The date the invoice was created.
        /// </summary>
        DateTime DateCreated { get; set; }
        /// <summary>
        /// The date upon which the invoice should be paid.
        /// </summary>
        DateTime DateDue { get; set; }
        /// <summary>
        /// Information about the invoice.
        /// </summary>
        string Details { get; set; }
        /// <summary>
        /// The discount, in percentage applied to the invoice, e.g 0.2.
        /// </summary>
        decimal Discount { get; set; }
        /// <summary>
        /// The total with the discount applied.
        /// </summary>
        decimal DiscountedTotal { get; }
        /// <summary>
        /// The unique ID of the invoice.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// The numerical index of the invoice.
        /// </summary>
        int Index { get; set; }
        /// <summary>
        /// The items comprising the invoice.
        /// </summary>
        ICollection<IPlutoInvoiceItem> Items { get; set; }
        /// <summary>
        /// The amount already paid
        /// </summary>
        decimal Paid { get; set; }
        /// <summary>
        /// The person billing the invoice.
        /// </summary>
        string Sender { get; set; }
        /// <summary>
        /// Shipping amount applied
        /// </summary>
        decimal Shipping { get; set; }
        /// <summary>
        /// Total of the items only
        /// </summary>
        decimal SubTotal { get; }
        /// <summary>
        /// Additional information of the invoice, applied as a tag.
        /// </summary>
        string Tag { get; set; }
        /// <summary>
        /// The tax applied in percentage, e.g 0.3 for 30%
        /// </summary>
        decimal Tax { get; set; }
        /// <summary>
        /// The total with the discount, tax and shipping applied.
        /// </summary>
        decimal Total { get; }
        /// <summary>
        /// Returns a transaction representing the invoice.
        /// </summary>
        /// <returns></returns>
        IPlutoTransaction GetTransaction();
        /// <summary>
        /// Returns a transaction representing the invoice, with the custom details.
        /// </summary>
        IPlutoTransaction GetTransaction(string details);
        /// <summary>
        /// Returns a transaction representing the invoie, with the custom details and type.
        /// </summary>
        IPlutoTransaction GetTransaction(string details, TransactionType type);
        /// <summary>
        /// Returns a transaction representing the invoice with the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IPlutoTransaction GetTransaction(TransactionType type);
    }
}