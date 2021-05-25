using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Thismaker.Thoth
{
    public class BindingItem : LocalizationBindingBase
    {
        public string Id { get; set; }
        public string TableKey { get; set; }
        public string ItemKey { get; set; }

        public void SetString()
        {
            var val = LocalizationManager.GetLocalizedString(TableKey, ItemKey);
            if (BindingTargets == null || BindingTargets.Count == 0)
            {
                var prop = Target.GetType().GetProperty(PropertyName);
                prop.SetValue(Target, val);
            }
            else
            {
                //construct the string:
                var args = new List<object>();

                foreach (var item in BindingTargets)
                {
                    args.Add(Target.GetPropValue(item.PropertyName));
                }

                var constructed = string.Format(val, args.ToArray());

                var prop = Target.GetType().GetProperty(PropertyName);
                prop.SetValue(Target, constructed);
            }
        }

        public BindingItem BindObservableTarget(INotifyPropertyChanged target, string propertyName, bool listen = true)
        {
            var inst = BindTarget(target, propertyName);
            var tar = BindingTargets.Find(x => x.Target == target);
            tar.ListenStatusChanged += TargetListenStatusChanged;
            tar.Listen = listen;
            tar.IsObservable = true;
            return inst;
        }

        private void TargetListenStatusChanged(BindingTarget target, bool status)
        {
            if (status)
            {
                try
                {
                    ((INotifyPropertyChanged)target.Target).PropertyChanged += TargetPropertyChanged;
                }
                catch
                {
                    //do nothing
                }
            }
            else
            {
                try
                {
                    ((INotifyPropertyChanged)target.Target).PropertyChanged -= TargetPropertyChanged;
                }
                catch
                {
                    //do nothing
                }
            }
        }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetString();
        }

        public void UnbindAllTargets()
        {
            if (BindingTargets == null) return;
            foreach (var tar in BindingTargets)
            {
                if (tar.IsObservable)
                {
                    tar.Listen = false;
                    tar.ListenStatusChanged -= TargetListenStatusChanged;
                }
            }
            BindingTargets.Clear();
        }

        public void UnbindTarget(object target, string propertyName)
        {
            var bindingTarget = BindingTargets.Find(x => x.Target == target && x.PropertyName == propertyName);

            if (bindingTarget == null) throw new NullReferenceException("Target not found");
            UnbindTarget(bindingTarget);
        }

        public void UnbindTarget(BindingTarget bindingTarget)
        {
            if (bindingTarget.IsObservable)
            {
                bindingTarget.Listen = false;
                bindingTarget.ListenStatusChanged -= TargetListenStatusChanged;
            }

            BindingTargets.Remove(bindingTarget);
        }

        public BindingItem BindTarget(object target, string propertyName)
        {
            if (BindingTargets == null)
            {
                BindingTargets = new List<BindingTarget>();
            }

            var item = new BindingTarget
            {
                PropertyName = propertyName,
                Target = target,
                IsObservable = false
            };

            BindingTargets.Add(item);

            return this;
        }

        public List<BindingTarget> BindingTargets { get; set; }
    }

    public abstract class LocalizationBindingBase
    {
        public object Target { get; set; }
        public string PropertyName { get; set; }
    }

    public class BindingTarget : LocalizationBindingBase
    {
        public event Action<BindingTarget, bool> ListenStatusChanged;

        private bool _listen;

        public bool Listen
        {
            get => _listen;
            set
            {
                _listen = value;
                ListenStatusChanged?.Invoke(this, Listen);
            }
        }
        public bool IsObservable { get; internal set; }
    }
}
