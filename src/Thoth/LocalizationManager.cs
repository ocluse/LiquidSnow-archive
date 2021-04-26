using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Thismaker.Horus.IO;

namespace Thismaker.Thoth
{
    public static class LocalizationManager
    {
        public static List<Locale> Locales { get; private set; }
        public static event Action<string> LocaleChanged;
        public static event Action<string> DefaultTableChanged;
        private static Locale _currentLocale;
        private static string _defaultTableKey;

        public static Dictionary<string, LocalizationTable> Tables { get; private set; }

        public static Locale CurrentLocale
        {
            get { return _currentLocale; }
            set
            {
                _currentLocale = value;
                OnCurrentLocaleChanged();
            }
        }

        public static string DefaultTableKey
        {
            get { return _defaultTableKey; }
            set
            {
                _defaultTableKey = value;
                DefaultTableChanged?.Invoke(_defaultTableKey);
            }
        }

        public static async Task LoadData(string path)
        {
            using var fsContainer = File.OpenRead(path);
            await LoadData(fsContainer);
        }

        public static async Task LoadData(Stream stream)
        {
            var data = await LocalizationData.LoadAsync(stream);
            Tables = new Dictionary<string, LocalizationTable>(data.Tables);
            DefaultTableKey = data.DefaultTableKey;
            CurrentLocale = data.Locales.Find(x => x.Id == data.DefaultLocaleId);
            Locales = new List<Locale>(data.Locales);
        }

        public static string GetLocalizedString(string key)
        {
            return GetLocalizedString(DefaultTableKey, key);
        }

        public static string GetLocalizedString(string table, string key)
        {
            return Tables[table].Items[key].Translations[CurrentLocale.Id];
        }

        private static List<BindingItem> _bindingItems;

        public static BindingItem BindProperty(object target, string propertyName, string table, string key)
        {
            var bindingItem = new BindingItem
            {
                Target = target,
                ItemKey = key,
                Id = Horus.Horus.GenerateID(),
                PropertyName = propertyName,
                TableKey = table
            };

            if (_bindingItems == null)
            {
                _bindingItems = new List<BindingItem>();

            }
            _bindingItems.Add(bindingItem);
            return bindingItem;
        }

        public static BindingItem ChangeBindingKey(string bindingId, string newKey, bool callSetString=false)
        {
            var item = _bindingItems.Find(x => x.Id == bindingId);
            return ChangeBindingKey(bindingId,  item.TableKey, newKey, callSetString);
        }

        public static BindingItem ChangeBindingKey(string bindingId, string newTable, string newKey, bool callSetString = false)
        {
            var item = _bindingItems.Find(x => x.Id == bindingId);
            item.ItemKey = newKey;
            item.TableKey = newTable;


            if (callSetString)
            {
                var val = GetLocalizedString(item.TableKey, item.ItemKey);
                item.SetString();
            }

            return item;
        }

        private static async void OnCurrentLocaleChanged()
        {
            LocaleChanged?.Invoke(CurrentLocale.ShortName);

            if (_bindingItems != null)
            {
                await SetBoundProperties();
            }
        }

       private static Task SetBoundProperties()
       {
            foreach (var item in _bindingItems)
            {
                
                item.SetString();
            }
            return Task.CompletedTask;
       }
    }

    public class BindingItem : LocalizationBindingBase
    {
        public string Id { get; set; }
        public string TableKey { get; set; }
        public string ItemKey { get; set; }

        public void SetString()
        {
            var val = LocalizationManager.GetLocalizedString(TableKey, ItemKey);
            if (BindingTargets == null)
            {
                var prop = Target.GetType().GetProperty(PropertyName);
                prop.SetValue(Target, val);
            }
            else
            {
                //construct the string:
                var args = new List<object>();

                foreach(var item in BindingTargets)
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
            var bindingTarget=BindTarget(target, propertyName, listen);

            if (listen)
            {
                target.PropertyChanged += TargetPropertyChanged;
            }
            return bindingTarget;
        }

        private void TargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetString();
        }

        public BindingItem BindTarget(object target, string propertyName, bool listen=false)
        {
            if (BindingTargets == null)
            {
                BindingTargets = new List<BindingTarget>();
            }

            var item = new BindingTarget
            {
                PropertyName = propertyName,
                Target = target,
                Listen=listen
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

    public class BindingTarget:LocalizationBindingBase
    {
        public event Action<BindingTarget, bool> ListenStatusChanged;

        private bool _listen;

        public bool Listen
        {
            get => _listen;
            set
            {
                _listen = value;
                ListenStatusChanged?.Invoke(this,Listen);
            }
        }
    }
}
