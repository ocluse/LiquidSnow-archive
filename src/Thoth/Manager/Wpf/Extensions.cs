using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Thismaker.Thoth.Manager.Wpf
{
    static class Extensions
    {
        public static LocTable ToMTable(this LocalizationTable table, string key)
        {
            LocTable locTable = new()
            {
                Items = new ObservableCollection<LocItem>(),
                TableKey = key,
            };

            foreach (KeyValuePair<string, LocalizationItem> item in table.Items)
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

            return locTable;
        }


        public static LocTable ToMTable(this KeyValuePair<string, LocalizationTable> entry)
        {
            return entry.Value.ToMTable(entry.Key);
        }

    }
}