using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Thismaker.Thoth
{
    /// <summary>
    /// The core class for updating, binding and loading locales to be used environment wide.
    /// </summary>
    public static class LocalizationManager
    {
        private static Locale _currentLocale;
        private static string _defaultTableKey;
        private static List<BindingItem> _bindingItems;

        /// <summary>
        /// The list of currently loaded locales
        /// </summary>
        public static List<Locale> Locales { get; private set; }
        
        /// <summary>
        /// An event fired when the current locale changes
        /// </summary>
        public static event Action<string> LocaleChanged;
        
        /// <summary>
        /// An event fired when the default table changes
        /// </summary>
        public static event Action<string> DefaultTableChanged;
        
        /// <summary>
        /// The list of all the tables that have been currently loaded
        /// </summary>
        public static Dictionary<string, LocalizationTable> Tables { get; private set; }

        /// <summary>
        /// Gets or sets the current locale, changing the value updates all bound items
        /// </summary>
        public static Locale CurrentLocale
        {
            get => _currentLocale;
            set
            {
                _currentLocale = value;
                OnCurrentLocaleChanged();
            }
        }

        /// <summary>
        /// Gets or sets the default table.
        /// </summary>
        public static string DefaultTableKey
        {
            get => _defaultTableKey;
            set
            {
                _defaultTableKey = value;
                DefaultTableChanged?.Invoke(_defaultTableKey);
            }
        }

        /// <summary>
        /// Loads locale data.
        /// </summary>
        /// <param name="data">The data to be loaded</param>
        /// <remarks>
        /// This function should be called only onces at the programs initialization.
        /// Calling the function will override all exisint tables and locales and definitely break binding.
        /// </remarks>
        public static void LoadData(LocalizationData data)
        {
            Tables = new Dictionary<string, LocalizationTable>(data.Tables);
            DefaultTableKey = data.DefaultTableKey;
            CurrentLocale = data.Locales.Find(x => x.ShortName == data.DefaultLocale);
            Locales = new List<Locale>(data.Locales);
        }

        /// <summary>
        /// Adds a sideload table to the list of tables, verifing if the table is compatible with the current data
        /// </summary>
        /// <param name="table">The table to be loaded</param>
        /// <exception cref="InvalidOperationException"></exception>
        public static void AddSideloadTable(SideloadTable table)
        {
            foreach (Locale locale in Locales)
            {
                if (!table.Locales.Exists(x => x.ShortName == locale.ShortName))
                {
                    throw new InvalidOperationException($"Sideload table missing locale with Id {locale.ShortName}");
                }
            }

            Tables.Add(table.Key, table);
        }

        /// <summary>
        /// Removes a previously added sideload table, unbinding targets subscribed to it in the process
        /// </summary>
        /// <param name="table">The table to be removed</param>
        public static void RemoveSideloadTable(SideloadTable table)
        {
            if (!Tables.ContainsKey(table.Key))
            {
                return;
            }

            //Unbind all properties with this key:
            if (_bindingItems != null)
            {
                List<BindingItem> removable = _bindingItems.FindAll(x => x.TableKey == table.Key);

                foreach (BindingItem item in removable)
                {
                    UnbindProperty(item);
                }
            }
            _ = Tables.Remove(table.Key);
        }

        /// <summary>
        /// Gets a string translation in the current locale from the default table
        /// </summary>
        /// <param name="key">The key of the item</param>
        public static string GetLocalizedString(string key)
        {
            return GetLocalizedString(DefaultTableKey, key);
        }

        /// <summary>
        /// Gets a string translation from the specified table in the current locale
        /// </summary>
        /// <param name="table">The key of the table</param>
        /// <param name="key">The key of the item in the table</param>
        public static string GetLocalizedString(string table, string key)
        {
            return Tables[table].Items[key].Translations[CurrentLocale.ShortName];
        }

        /// <summary>
        /// Binds the property of an object to a localization item
        /// </summary>
        /// <param name="target">The object to be bound</param>
        /// <param name="propertyName">The name of the object's property to be bound</param>
        /// <param name="table">The ey of the table</param>
        /// <param name="key">The key of the item in the table</param>
        /// <param name="update">If true, changing the locale will result in updating the property value</param>
        /// <returns>The binding item representing the binding that can be used to add other objects if they key is the same</returns>
        public static BindingItem BindProperty(object target, string propertyName, string table, string key, bool update = true)
        {
            BindingItem bindingItem = new BindingItem
            {
                Target = target,
                ItemKey = key,
                Id = Horus.Horus.GenerateId(),
                PropertyName = propertyName,
                TableKey = table
            };

            if (_bindingItems == null)
            {
                _bindingItems = new List<BindingItem>();

            }
            _bindingItems.Add(bindingItem);

            //set the value immediate:
            if (update)
            {
                bindingItem.SetString();
            }

            return bindingItem;
        }

        /// <summary>
        /// Unbinds a previously bound property
        /// </summary>
        public static void UnbindProperty(object target, string propertyName, string table, string key)
        {
            BindingItem item = _bindingItems.Find(x => x.Target == target &&
            x.PropertyName == propertyName &&
            x.TableKey == table &&
            x.ItemKey == key);

            if (item == null)
            {
                throw new InvalidOperationException("Target not found");
            }

            UnbindProperty(item);
        }

        /// <summary>
        /// Unbinds a previously bound property
        /// </summary>
        public static void UnbindProperty(BindingItem item)
        {
            if (!_bindingItems.Contains(item))
            {
                throw new ArgumentException("The provided binding item was not found in the current localization manager");
            }

            item.UnbindAllTargets();
            _ = _bindingItems.Remove(item);
        }

        /// <summary>
        /// Unbinds all properties of all objects
        /// </summary>
        public static void UnbindAllProperties()
        {
            if (_bindingItems == null)
            {
                return;
            }

            foreach (BindingItem item in _bindingItems)
            {
                item.UnbindAllTargets();
            }
            _bindingItems.Clear();
        }

        /// <summary>
        /// Changes the item key without changing the table
        /// </summary>
        public static BindingItem ChangeBindingKey(string bindingId, string newKey, bool update = true, bool unbindTargets = true)
        {
            BindingItem item = _bindingItems.Find(x => x.Id == bindingId);
            return ChangeBindingKey(item, item.TableKey, newKey, update, unbindTargets);
        }

        /// <summary>
        /// Changes the item key and the table key
        /// </summary>
        public static BindingItem ChangeBindingKey(string bindingId, string newTable, string newKey, bool update = true, bool unbindTargets = true)
        {
            BindingItem item = _bindingItems.Find(x => x.Id == bindingId);
            return ChangeBindingKey(item, newKey, newTable, update, unbindTargets);
        }

        /// <summary>
        /// Changes the item key without changing the table
        /// </summary>
        public static BindingItem ChangeBindingKey(BindingItem bindingItem, string newKey, bool update = true, bool unbindTargets = true)
        {
            return ChangeBindingKey(bindingItem, bindingItem.TableKey, newKey, update, unbindTargets);
        }

        /// <summary>
        /// Changes the item key and the table key
        /// </summary>
        public static BindingItem ChangeBindingKey(BindingItem bindingItem, string newTable, string newKey, bool update = true, bool unbindTargets = true)
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
            foreach (BindingItem item in _bindingItems)
            {
                item.SetString();
            }
            return Task.CompletedTask;
        }
    }
}
