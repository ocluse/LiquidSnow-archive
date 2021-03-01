using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    public class Invoice : BindableBase
    {
        #region Private Fields
        private ObservableCollection<InvoiceItem> _items;
        private DateTime _dateDue, _dateCreated;
        private string _id, _details, _tag, _billedTo;
        private string _sender;
        private double _tax;
        private double? _shipping, _discount;
        private double _paid;
        #endregion

        #region Initialiation
        public Invoice()
        {
            DateCreated = DateTime.UtcNow;
            ID = Horus.Horus.GenerateID();
        }
        #endregion

        #region Properties
        /// <summary>
        /// The unique identifier of the invoice
        /// </summary>
        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        
        /// <summary>
        /// The details or description of the invoice
        /// </summary>
        public string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }
        
        /// <summary>
        /// The tag of the invoice
        /// </summary>
        public string Tag
        {
            get => _details;
            set => SetProperty(ref _tag, value);
        }
        
        /// <summary>
        /// The addresee of the invoice
        /// </summary>
        public string BilledTo
        {
            get=> _billedTo;
            set => SetProperty(ref _billedTo, value);
        }
        
        /// <summary>
        /// The person sending/creating the invoice
        /// </summary>
        public string Sender
        {
            get => _sender;
            set => SetProperty(ref _sender, value);
        }
        
        /// <summary>
        /// The tax applied to the invoice, expressed as a 0-1 percentage. i.e if 0.3 then tax is 30%
        /// </summary>
        public double Tax
        {
            get => _tax;
            set => SetProperty(ref _tax, value);
        }

        /// <summary>
        /// A list of items in the invoice
        /// </summary>
        public ObservableCollection<InvoiceItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        
        /// <summary>
        /// The due date of the invoice i.e the date it is to be paid
        /// </summary>
        public DateTime DateDue
        {
            get => _dateDue;
            set => SetProperty(ref _dateDue, value);
        }
        
        /// <summary>
        /// The date the invoice was created.
        /// </summary>
        public DateTime DateCreated
        {
            get => _dateCreated;
            set => SetProperty(ref _dateCreated, value);
        }
        
        /// <summary>
        /// The total shipping amount. If provided.
        /// </summary>
        public double? Shipping
        {
            get => _shipping;
            set => SetProperty(ref _shipping, value);
        }
        
        /// <summary>
        /// The discount to charge on the items as a percentage of 0-1, ie 25% discount is 0.25
        /// </summary>
        public double? Discount
        {
            get => _discount;
            set => SetProperty(ref _discount, value);
        }
        
        /// <summary>
        /// The total currently paid amount.
        /// </summary>
        public double Paid
        {
            get => _paid;
            set => SetProperty(ref _paid, value);
        }

        /// <summary>
        /// This the total amount of items in the invoice excluding tax, discounts and shipping costs
        /// </summary>
        public double SubTotal
        {
            get
            {
                if (Items == null) return 0;
                if (Items.Count == 0) return 0;
                double totals = 0;
                
                foreach(var item in Items)
                {
                    totals += item.Totals;
                }

                return totals;
            }
        }

        /// <summary>
        /// This is the total amount of the invoice, including taxes, discounts and shippings costs
        /// </summary>
        public double Total
        {
            get
            {
                //Get discounted total
                var totals = DiscountedTotal;

                //Add tax:
                var tax = Tax * totals;
                totals += tax;

                //Add shipping:
                if (Shipping.HasValue) totals += Shipping.Value;

                //Return subtotal
                return totals;
            }
        }

        /// <summary>
        /// The totals of the invoice including discounts
        /// </summary>
        public double DiscountedTotal
        {
            get
            {
                var totals = SubTotal;
                if (!Discount.HasValue) return totals;

                //compute discount:
                var discount = totals * Discount.Value;
                totals -= discount;

                return totals;
            }
        }

        /// <summary>
        /// The difference between the <see cref="Total"/> amount and the <see cref="Paid"/> amount
        /// </summary>
        public double Balance
        {
            get => Total-Paid;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a transaction that can be used to represent the invoice in a <see cref="Account"/>
        /// </summary>
        /// <returns>A <see cref="Transaction"/> that is debit in nature, with the same details as invoice</returns>
        public Transaction GetTransaction()
        {
            return GetTransaction(Details);
        }

        /// <summary>
        /// Returns a transaction that can be used to represent the invoice in a <see cref="Account"/>
        /// </summary>
        /// <param name="details">The custom details to use with the transaction</param>
        /// <returns>A <see cref="Transaction"/> that is debit in nature, with the provided details</returns>
        public Transaction GetTransaction(string details)
        {
            return GetTransaction(details, TransactionType.Debit);
        }

        /// <summary>
        /// Returns a transaction that can be used to represent the invoice in a <see cref="Account"/>
        /// </summary>
        /// <param name="details">The custom details to use with the transaction</param>
        /// <param name="type">The custom type to assign to the transaction</param>
        /// <returns>A <see cref="Transaction"/> that has the provided nature, with the provided details</returns>
        public Transaction GetTransaction(string details, TransactionType type)
        {
            //first get subtotal:
            return new Transaction()
            {
                Amount = Total,
                Date = DateCreated,
                Tag = ID,
                Details = details,
                Type = type
            };
        }

        #endregion
    }

    public class InvoiceItem:Charge
    {
        private double _quantity;

        public double Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }
        public double Totals
        {
            get => _quantity * Amount;
        }

        //Call our base constructor to assign an auto id
        public InvoiceItem() : base()
        {

        }
    }
}
