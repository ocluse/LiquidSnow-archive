namespace Thismaker.Pluto
{
    public interface IPlutoTransaction : IPlutoCharge
    {
        TransactionType Type { get; set; }
    }
}