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
            var table = new LocalizationTable
            {
                Items = new Dictionary<string, LocalizationItem>()
            };

            foreach(var item in Items)
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
            var item = new LocalizationItem
            {
                Translations = new Dictionary<string, string>()
            };

            foreach(var trans in Translations)
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
        private string _id, _name, _shortName;

        public string ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

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
                Id = ID,
                Name = Name,
                ShortName = ShortName
            };
        }
    }

    public static class ModelExtensions
    {
        public static IEnumerable<ManagedLocale> GetManagedLocales(this LocalizationData data)
        {
            var result = new List<ManagedLocale>();

            foreach(var locale in data.Locales)
            {
                var manLocale = new ManagedLocale
                {
                    ID = locale.Id,
                    Name = locale.Name,
                    ShortName = locale.ShortName
                };

                result.Add(manLocale);
            }

            return result;
        }

        public static IEnumerable<LocTable> GetLocTables(this LocalizationData data)
        {
            var result = new List<LocTable>();

            foreach(var table in data.Tables)
            {
                var locTable = new LocTable
                {
                    Items = new ObservableCollection<LocItem>(),
                    TableKey = table.Key,
                };

                foreach(var item in table.Value.Items)
                {
                    var locItem = new LocItem
                    {
                        Key = item.Key,
                        Translations = new ObservableCollection<LocTranslation>()
                    };

                    foreach(var trans in item.Value.Translations)
                    {
                        var locTrans = new LocTranslation
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
            foreach(var locTable in tables)
            {
                var table = locTable.ToTable();
                data.Tables.Add(locTable.TableKey, table);
            }
        }

        public static void SetManagedLocales(this LocalizationData data, IEnumerable<ManagedLocale> locales)
        {
            data.Locales = new List<Locale>();
            foreach(var manLocale in locales)
            {
                data.Locales.Add(manLocale.ToLocale());
            }
        }
    }
}