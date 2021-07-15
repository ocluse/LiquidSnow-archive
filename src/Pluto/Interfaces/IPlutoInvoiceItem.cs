namespace Thismaker.Pluto
{
    public interface IPlutoInvoiceItem : IPlutoCharge
    {
        decimal Quantity { get; set; }
        decimal Totals { get; }
    }
}