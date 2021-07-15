using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    /// <summary>
    /// An invoice represents a list of items charged against an entity
    /// </summary>
    public class PlutoInvoice : BindableBase, IPlutoInvoice
    {
        #region Private Fields
        private ICollection<IPlutoInvoiceItem> _items;
        private DateTime _dateDue, _dateCreated;
        private string _id, _details, _tag, _billedTo;
        private string _sender;
        private decimal _tax;
        private decimal _shipping, _discount;
        private decimal _paid;
        private int _index;
        #endregion

        #region Properties

        /// <summary>
        /// The index of the <see cref="PlutoInvoice"/>. Usually the number of the invoice.
        /// In business we say e.g Invoice No 23.
        /// </summary>
        public virtual int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }

        /// <summary>
        /// The unique identifier of the invoice
        /// </summary>
        public virtual string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// The details or description of the invoice
        /// </summary>
        public virtual string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }

        /// <summary>
        /// The tag of the invoice
        /// </summary>
        public virtual string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }

        /// <summary>
        /// The addresee of the invoice
        /// </summary>
        public virtual string BilledTo
        {
            get => _billedTo;
            set => SetProperty(ref _billedTo, value);
        }

        /// <summary>
        /// The person sending/creating the invoice
        /// </summary>
        public virtual string Sender
        {
            get => _sender;
            set => SetProperty(ref _sender, value);
        }

        /// <summary>
        /// The tax applied to the invoice, expressed as a 0-1 percentage. i.e if 0.3 then tax is 30%
        /// </summary>
        public virtual decimal Tax
        {
            get => _tax;
            set => SetProperty(ref _tax, value);
        }

        /// <summary>
        /// A list of items in the invoice
        /// </summary>
        public virtual ICollection<IPlutoInvoiceItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        /// <summary>
        /// The due date of the invoice i.e the date it is to be paid
        /// </summary>
        public virtual DateTime DateDue
        {
            get => _dateDue;
            set => SetProperty(ref _dateDue, value);
        }

        /// <summary>
        /// The date the invoice was created.
        /// </summary>
        public virtual DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }

        /// <summary>
        /// The total shipping amount. If provided.
        /// </summary>
        public virtual decimal Shipping
        {
            get => _shipping;
            set => SetProperty(ref _shipping, value);
        }

        /// <summary>
        /// The discount to charge on the items as a percentage of 0-1, ie 25% discount is 0.25
        /// </summary>
        public virtual decimal Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }

        /// <summary>
        /// The total currently paid amount.
        /// </summary>
        public virtual decimal Paid
        {
            get => _paid;
            set => SetProperty(ref _paid, value);
        }

        /// <summary>
        /// This the total amount of items in the invoice excluding tax, discounts and shipping costs
        /// </summary>
        public virtual decimal SubTotal
        {
            get
            {
                if (Items == null) return 0;
                if (Items.Count == 0) return 0;
                decimal totals = 0;

                foreach (var item in Items)
                {
                    totals += item.Totals;
                }

                return totals;
            }
        }

        /// <summary>
        /// This is the total amount of the invoice, including taxes, discounts and shippings costs
        /// </summary>
        public virtual decimal Total
        {
            get
            {
                //Get discounted total
                var totals = DiscountedTotal;

                //Add tax:
                var tax = Tax * totals;
                totals += tax;

                //Add shipping:
                totals += Shipping;

                //Return subtotal
                return totals;
            }
        }

        /// <summary>
        /// The totals of the invoice including discounts
        /// </summary>
        public virtual decimal DiscountedTotal
        {
            get
            {
                var totals = SubTotal;
                //compute discount:
                var discount = totals * Discount;
                totals -= discount;

                return totals;
            }
        }

        /// <summary>
        /// The difference between the <see cref="Total"/> amount and the <see cref="Paid"/> amount
        /// </summary>
        public virtual decimal Balance
        {
            get => Total - Paid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a transaction that can be used to represent the invoice in a <see cref="PlutoAccount"/>
        /// </summary>
        /// <returns>A <see cref="PlutoTransaction"/> that is debit in nature, with the same details as invoice</returns>
        public virtual IPlutoTransaction GetTransaction()
        {
            return GetTransaction(Details);
        }

        /// <summary>
        /// Returns a transaction that can be used to represent the invoice in a <see cref="PlutoAccount"/>
        /// </summary>
        /// <param name="details">The custom details to use with the transaction</param>
        /// <returns>A <see cref="PlutoTransaction"/> that is debit in nature, with the provided details</returns>
        public virtual IPlutoTransaction GetTransaction(string details)
        {
            return GetTransaction(details, TransactionType.Debit);
        }

        /// <summary>
        /// Returns a transaction that can be used to re[resent the invoice in a <see cref="PlutoAccount"/>,
        /// using the <see cref="Details"/> of the invoice as the details of the transaction
        /// </summary>
        /// <param name="type">They type of trasaction, whether credit or debit</param>
        /// <returns></returns>
        public virtual IPlutoTransaction GetTransaction(TransactionType type)
        {
            return GetTransaction(Details, type);
        }

        /// <summary>
        /// Returns a transaction that can be used to represent the invoice in a <see cref="PlutoAccount"/>
        /// </summary>
        /// <param name="details">The custom details to use with the transaction</param>
        /// <param name="type">The custom type to assign to the transaction</param>
        /// <returns>A <see cref="PlutoTransaction"/> that has the provided nature, with the provided details</returns>
        public virtual IPlutoTransaction GetTransaction(string details, TransactionType type)
        {
            //first get subtotal:
            return new PlutoTransaction()
            {
                Amount = Total,
                Date = DateCreated,
                Tag = Id,
                Details = details,
                Type = type
            };
        }

        /// <summary>
        /// Copies the properties of the source invoice into the current invoice
        /// </summary>
        /// <param name="source"></param>
        public virtual void Update(IPlutoInvoice source)
        {
            Index = source.Index;
            Id = source.Id;
            Details = source.Details;
            Tag = source.Tag;
            BilledTo = source.BilledTo;
            Sender = source.Sender;
            Tax = source.Tax;
            DateDue = source.DateDue;
            DateCreated = source.DateCreated;
            Shipping = source.Shipping;
            Discount = source.Discount;
            Paid = source.Paid;
            Items = source.Items;
        }

        #endregion
    }
}