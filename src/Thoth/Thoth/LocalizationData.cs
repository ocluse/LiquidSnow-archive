using System.Collections.Generic;

namespace Thismaker.Thoth
{
    public class LocalizationData
    {
        public Dictionary<string, LocalizationTable> Tables { get; set; }

        public List<Locale> Locales { get; set; }

        public string DefaultLocale { get; set; }

        public string DefaultTableKey { get; set; }
    }
}