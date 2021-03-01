using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Thismaker.Core;

namespace Thismaker.Pluto
{
    public class Charge : BindableBase
    {
        private string _id, _details, _tag;
        private double _amount;
        private DateTime _date;
        
        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public string Details
        {
            get => _details;
            set => SetProperty(ref _details, value);
        }
        public string Tag
        {
            get => _tag;
            set => SetProperty(ref _tag, value);
        }
        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public Charge()
        {
            ID = Horus.Horus.GenerateID();
            Date = DateTime.UtcNow;
        }

        public static Charge FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Charge>(json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
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
