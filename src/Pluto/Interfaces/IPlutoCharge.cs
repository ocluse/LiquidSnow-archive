using System;

namespace Thismaker.Pluto
{
    public interface IPlutoCharge
    {
        decimal Amount { get; set; }
        DateTime Date { get; set; }
        string Details { get; set; }
        string Id { get; set; }
        string Tag { get; set; }
    }
}