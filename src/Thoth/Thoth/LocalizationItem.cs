using System.Collections.Generic;

namespace Thismaker.Thoth
{
    /// <summary>
    /// A class definining locales and the translations of the items in those locales
    /// </summary>
    public class LocalizationItem
    {
        /// <summary>
        /// A dictionary whose key is the locale shortname and value the translation in that locale
        /// </summary>
        public Dictionary<string, string> Translations { get; set; }
    }
}
