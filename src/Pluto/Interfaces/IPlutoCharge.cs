using System;

namespace Thismaker.Pluto
{
    /// <summary>
    /// Base for charge items.
    /// </summary>
    public interface IPlutoCharge
    {
        /// <summary>
        /// The amount in the charge.
        /// </summary>
        decimal Amount { get; set; }
        /// <summary>
        /// The date the charge is created.
        /// </summary>
        DateTime Date { get; set; }
        /// <summary>
        /// The details of the charge.
        /// </summary>
        string Details { get; set; }
        /// <summary>
        /// The unique Id of the charge.
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// The tag of the charge.
        /// </summary>
        string Tag { get; set; }
    }
}