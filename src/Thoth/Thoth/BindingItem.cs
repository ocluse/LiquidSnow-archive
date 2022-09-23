using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Thismaker.Thoth
{
    /// <summary>
    /// A class with utility methods for binding objects to a locale item
    /// and updating them when the locale changes.
    /// </summary>
    public class BindingItem : LocalizationBindingBase
    {
        /// <summary>
        /// Gets or sets the unique identifier of the binding item
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// Gets or set the key of the table to search for binding.
        /// </summary>
        public string TableKey { get; set; }
        
        /// <summary>
        /// Gets or sets the key of the locale item to be bound to.
        /// </summary>
        public string ItemKey { get; set; }

        /// <summary>
        /// The list of targets bound to the current locale item
        /// </summary>
        public List<BindingTarget> BindingTargets { get; set; }

        /// <summary>
        /// Sets or updates the targeted object's property, respecting any formatting provided.
        /// </summary>
        public void SetString()
        {
            string val = LocalizationManager.GetLocalizedString(TableKey, ItemKey);
            if (BindingTargets == null || BindingTargets.Count == 0)
            {
                System.Reflection.PropertyInfo prop = Target.GetType().GetProperty(PropertyName);
                prop.SetValue(Target, val);
            }
            else
            {
                //construct the string:
                List<object> args = new List<object>();

                foreach (BindingTarget item in BindingTargets)
                {
                    args.Add(Target.GetPropValue(item.PropertyName));
                }

                string constructed = string.Format(val, args.ToArray());

                System.Reflection.PropertyInfo prop = Target.GetType().GetProperty(PropertyName);
                prop.SetValue(Target, constructed);
            }
        }

        /// <summary>
        /// Binds the property of the <paramref name="target"/> to the locale item.
        /// </summary>
        /// <param name="target">The object whose property is to be bound</param>
        /// <param name="propertyName">The name of the property to be bound</param>
        /// <returns>The current binding item with the target added</returns>
        public BindingItem BindTarget(object target, string propertyName)
        {
            if (BindingTargets == null)
            {
                BindingTargets = new List<BindingTarget>();
            }

            BindingTarget item = new BindingTarget
            {
                PropertyName = propertyName,
                Target = target,
                IsObservable = false
            };

            BindingTargets.Add(item);

            return this;
        }

        /// <summary>
        /// Binds the property of an object implementing <see cref="INotifyPropertyChanged"/> to the current item,
        /// allowing for updating the property when a value changes in the object.
        /// </summary>
        /// <param name="target">The target object</param>
        /// <param name="propertyName">The name of the property to be bound</param>
        /// <param name="listen">Indicates whether to listen for property changes in the object</param>
        /// <returns></returns>
        public BindingItem BindObservableTarget(INotifyPropertyChanged target, string propertyName, bool listen = true)
        {
            BindingItem inst = BindTarget(target, propertyName);
            BindingTarget tar = BindingTargets.Find(x => x.Target == target);
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

        /// <summary>
        /// Removes all objects currently bound to the locale item
        /// </summary>
        public void UnbindAllTargets()
        {
            if (BindingTargets == null)
            {
                return;
            }

            foreach (BindingTarget tar in BindingTargets)
            {
                if (tar.IsObservable)
                {
                    tar.Listen = false;
                    tar.ListenStatusChanged -= TargetListenStatusChanged;
                }
            }
            BindingTargets.Clear();
        }

        /// <summary>
        /// Searches for the <see cref="BindingTarget"/> of the <paramref name="target"/> object
        /// and correctly unbinds it's property from the current binding item.
        /// </summary>
        /// <param name="target">The object who's property is to be unbinded</param>
        /// <param name="propertyName">The name of the property to be unbinded</param>
        /// <returns>True if the property was found and successfully removed</returns>
        public bool UnbindTarget(object target, string propertyName)
        {
            BindingTarget bindingTarget = BindingTargets.Find(x => x.Target == target && x.PropertyName == propertyName);

            return UnbindTarget(bindingTarget);
        }

        /// <summary>
        /// Correctly unbinds a <see cref="BindingTarget"/> from the current item.
        /// </summary>
        /// <param name="bindingTarget">The target to be removed</param>
        /// <returns>True if the target was found and successfuly removed</returns>
        public bool UnbindTarget(BindingTarget bindingTarget)
        {
            if (BindingTargets.Remove(bindingTarget))
            {
                if (bindingTarget.IsObservable)
                {
                    bindingTarget.Listen = false;
                    bindingTarget.ListenStatusChanged -= TargetListenStatusChanged;
                }

                return true;
            }

            return false;
        }
    }

    /// <summary>
    /// A base class used by <see cref="BindingTarget"/> and <see cref="BindingItem"/>
    /// </summary>
    public abstract class LocalizationBindingBase
    {
        /// <summary>
        /// Gets or sets the target object
        /// </summary>
        public object Target { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the property to be updated
        /// </summary>
        public string PropertyName { get; set; }
    }

    /// <summary>
    /// A class with utility methods for binding an object's property to a locale item
    /// </summary>
    public class BindingTarget : LocalizationBindingBase
    {
        /// <summary>
        /// Fired when the value of <see cref="Listen"/> changes. 
        /// </summary>
        public event Action<BindingTarget, bool> ListenStatusChanged;

        private bool _listen;
        
        /// <summary>
        /// Gets or sets a value determining whether the target's property will be updated 
        /// when a change occurs, for example, if the locale is changed.
        /// </summary>
        public bool Listen
        {
            get => _listen;
            set
            {
                _listen = value;
                ListenStatusChanged?.Invoke(this, Listen);
            }
        }

        /// <summary>
        /// A value indicating if the target object implements <see cref="INotifyPropertyChanged"/>
        /// </summary>
        public bool IsObservable { get; internal set; }
    }
}
