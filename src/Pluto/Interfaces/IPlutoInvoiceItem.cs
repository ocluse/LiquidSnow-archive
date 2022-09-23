namespace Thismaker.Pluto
{
    /// <summary>
    /// An single item representing an item added to the invoice.
    /// </summary>
    public interface IPlutoInvoiceItem : IPlutoCharge
    {
        /// <summary>
        /// The quantity of the item being charged.
        /// </summary>
        decimal Quantity { get; set; }
        /// <summary>
        /// The product of the quantity and the amount for a single unit.
        /// </summary>
        decimal Totals { get; }
    }
}