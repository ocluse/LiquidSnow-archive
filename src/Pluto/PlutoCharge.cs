using System;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    /// <summary>
    /// A charge simply represents a unit of money that is applied
    /// </summary>
    public class PlutoCharge : BindableBase, IPlutoCharge
    {
        private string _id, _details, _tag;
        private decimal _amount;
        private DateTime _date;

        /// <summary>
        /// The unique, preferably unchanging identifier of the charge
        /// </summary>
        public virtual string Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        /// <summary>
        /// A narrative that provides a reason why the charge was executed
        /// </summary>
        public virtual string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }

        /// <summary>
        /// Can be used to provide custom properties to the charge
        /// </summary>
        public virtual string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }

        /// <summary>
        /// The amount of money that actually represents the charge
        /// </summary>
        public virtual decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        /// <summary>
        /// The date and time on which the charge was executed
        /// </summary>
        public virtual DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        /// <summary>
        /// A simple way to update the vazz
        /// </summary>
        /// <param name="source"></param>
        public virtual void Update(IPlutoCharge source)
        {
            Date = source.Date;
            Amount = source.Amount;
            Details = source.Details;
            Id = source.Id;
            Tag = source.Tag;
        }
    }
}
