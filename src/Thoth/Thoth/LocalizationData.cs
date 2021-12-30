using System.Collections.Generic;

namespace Thismaker.Thoth
{
    /// <summary>
    /// A class containing definitions of locales and the various translations of different items in tables
    /// </summary>
    public class LocalizationData
    {
        /// <summary>
        /// Gets or sets the various tables containing the various items
        /// </summary>
        public Dictionary<string, LocalizationTable> Tables { get; set; }

        /// <summary>
        /// The list of locales whose translations are contained in the tables
        /// </summary>
        public List<Locale> Locales { get; set; }

        /// <summary>
        /// The shortname fallback locale
        /// </summary>
        public string DefaultLocale { get; set; }

        /// <summary>
        /// The key of the fallback table
        /// </summary>
        public string DefaultTableKey { get; set; }
    }
}