using System;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    public class Charge : BindableBase
    {
        private string _id, _details, _tag;
        private decimal _amount;
        private DateTime _date;
        
        public virtual string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public virtual string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }
        public virtual string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }
        public virtual decimal Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }
        public virtual DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public Charge()
        {
            ID = Horus.Horus.GenerateID();
            Date = DateTime.UtcNow;
        }

        public virtual void Update(Charge copy)
        {
            Date = copy.Date;
            Amount = copy.Amount;
            Details = copy.Details;
            ID = copy.ID;
            Tag = copy.Tag;
        }
    }
}
