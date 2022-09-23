using System.Collections.Generic;
using System.Collections.ObjectModel;
using Thismaker.Core;

namespace Thismaker.Thoth.Manager.Wpf
{
    public class LocTable : BindableBase
    {
        private string _tableKey;

        private ObservableCollection<LocItem> _items;

        public string TableKey
        {
            get => _tableKey;
            set => SetProperty(ref _tableKey, value);
        }

        public ObservableCollection<LocItem> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public LocalizationTable ToTable()
        {
            LocalizationTable table = new()
            {
                Items = new Dictionary<string, LocalizationItem>()
            };

            foreach (LocItem item in Items)
            {
                table.Items.Add(item.Key, item.ToItem());
            }

            return table;
        }
    }

    public class LocItem : BindableBase
    {
        public string _key;
        public ObservableCollection<LocTranslation> _items;


        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        public ObservableCollection<LocTranslation> Translations
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public LocalizationItem ToItem()
        {
            LocalizationItem item = new()
            {
                Translations = new Dictionary<string, string>()
            };

            foreach (LocTranslation trans in Translations)
            {
                item.Translations.Add(trans.Locale, trans.Value);
            }

            return item;
        }
    }

    public class LocTranslation : BindableBase
    {
        private string _value;
        private string _locale;

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public string Locale
        {
            get => _locale;
            set => SetProperty(ref _locale, value);
        }
    }

    public class ManagedLocale : BindableBase
    {
        private string _name, _shortName;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string ShortName
        {
            get => _shortName;
            set => SetProperty(ref _shortName, value);
        }

        public Locale ToLocale()
        {
            return new Locale
            {
                Name = Name,
                ShortName = ShortName
            };
        }
    }

    public static class ModelExtensions
    {
        public static IEnumerable<ManagedLocale> GetManagedLocales(this LocalizationData data)
        {
            List<ManagedLocale> result = new();

            foreach (Locale locale in data.Locales)
            {
                ManagedLocale manLocale = new()
                {
                    Name = locale.Name,
                    ShortName = locale.ShortName
                };

                result.Add(manLocale);
            }

            return result;
        }

        public static IEnumerable<LocTable> GetLocTables(this LocalizationData data)
        {
            List<LocTable> result = new();

            foreach (KeyValuePair<string, LocalizationTable> table in data.Tables)
            {
                LocTable locTable = new()
                {
                    Items = new ObservableCollection<LocItem>(),
                    TableKey = table.Key,
                };

                foreach (KeyValuePair<string, LocalizationItem> item in table.Value.Items)
                {
                    LocItem locItem = new()
                    {
                        Key = item.Key,
                        Translations = new ObservableCollection<LocTranslation>()
                    };

                    foreach (KeyValuePair<string, string> trans in item.Value.Translations)
                    {
                        LocTranslation locTrans = new()
                        {
                            Locale = trans.Key,
                            Value = trans.Value,
                        };
                        locItem.Translations.Add(locTrans);
                    }

                    locTable.Items.Add(locItem);
                }

                result.Add(locTable);
            }

            return result;
        }

        public static void SetLocTables(this LocalizationData data, IEnumerable<LocTable> tables)
        {
            data.Tables = new Dictionary<string, LocalizationTable>();
            foreach (LocTable locTable in tables)
            {
                LocalizationTable table = locTable.ToTable();
                data.Tables.Add(locTable.TableKey, table);
            }
        }

        public static void SetManagedLocales(this LocalizationData data, IEnumerable<ManagedLocale> locales)
        {
            data.Locales = new List<Locale>();
            foreach (ManagedLocale manLocale in locales)
            {
                data.Locales.Add(manLocale.ToLocale());
            }
        }
    }
}