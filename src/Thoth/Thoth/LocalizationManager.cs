﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public static void LoadData(LocalizationData data)
        {
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

        public static void UnbindProperty(object target, string propertyName, string table, string key)
        {
            var item = _bindingItems.Find(x => x.Target == target && 
            x.PropertyName == propertyName && 
            x.TableKey == table && 
            x.ItemKey == key);

            if (item == null) throw new NullReferenceException("Target not found");
            UnbindProperty(item);
        }

        public static void UnbindProperty(BindingItem item)
        {
            if (!_bindingItems.Contains(item)) throw new ArgumentException("The provided binding item was not found in the current localization manager");
            item.UnbindAllTargets();
            _bindingItems.Remove(item);
        }

        public static void UnbindAllProperties()
        {
            if (_bindingItems == null) return;
            foreach(var item in _bindingItems)
            {
                item.UnbindAllTargets();
            }
            _bindingItems.Clear();
        }

        public static BindingItem ChangeBindingKey(string bindingId, string newKey, bool update=true, bool unbindTargets=true)
        {
            var item = _bindingItems.Find(x => x.Id == bindingId);
            return ChangeBindingKey(item, item.TableKey, newKey, update, unbindTargets);
        }

        public static BindingItem ChangeBindingKey(string bindingId, string newTable, string newKey, bool update = false, bool unbindTargets=true)
        {
            var item = _bindingItems.Find(x => x.Id == bindingId);
            return ChangeBindingKey(item, newKey, newTable, update, unbindTargets);
        }

        public static BindingItem ChangeBindingKey(BindingItem bindingItem, string newKey, bool update = false, bool unbindTargets = true)
        {
            return ChangeBindingKey(bindingItem, bindingItem.TableKey, newKey, update, unbindTargets);
        }

        public static BindingItem ChangeBindingKey(BindingItem bindingItem, string newTable, string newKey, bool update = false, bool unbindTargets = true)
        {
            bindingItem.ItemKey = newKey;
            bindingItem.TableKey = newTable;

            if (unbindTargets)
            {
                bindingItem.UnbindAllTargets();
            }
            if (update)
            {
                bindingItem.SetString();
            }
            return bindingItem;
        }

        private static async void OnCurrentLocaleChanged()
        {
            if (_bindingItems != null)
            {
                await SetBoundProperties();
            }

            LocaleChanged?.Invoke(CurrentLocale.ShortName);
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
}