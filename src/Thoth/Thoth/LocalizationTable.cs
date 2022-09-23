using System.Collections.Generic;

namespace Thismaker.Thoth
{
    /// <summary>
    /// A table defining the localization items and their keys
    /// </summary>
    public class LocalizationTable
    {
        /// <summary>
        /// The dictionary of the items and their keys
        /// </summary>
        public Dictionary<string, LocalizationItem> Items { get; set; }
    }
}
