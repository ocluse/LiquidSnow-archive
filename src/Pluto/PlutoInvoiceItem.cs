using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Pluto
{
    /// <summary>
    /// An invoice item is an individual item within the invoice,
    /// which represents a the charges in relation to an item of purchase
    /// </summary>
    public class PlutoInvoiceItem : PlutoCharge, IPlutoInvoiceItem
    {
        private decimal _quantity;

        /// <summary>
        /// The quantity, e.g weight, number of each individual unit
        /// </summary>
        public virtual decimal Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        /// <summary>
        /// The total of the items, calulated by multipying quantity and amount
        /// </summary>
        public decimal Totals
        {
            get => _quantity * Amount;
        }

        public override void Update(IPlutoCharge source)
        {
            if (source.GetType().IsAssignableFrom(typeof(PlutoInvoiceItem)))
            {
                PlutoInvoiceItem derived = (PlutoInvoiceItem)source;
                Quantity = derived.Quantity;
                Amount = derived.Amount;
            }
            base.Update(source);
        }
    }
}
