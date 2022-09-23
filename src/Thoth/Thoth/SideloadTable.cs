using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Thoth
{
    /// <summary>
    /// A table that can be added after localization has already been loaded
    /// </summary>
    /// <remarks>
    /// Useful if the same locale definition is to be shared among different programs
    /// </remarks>
    public class SideloadTable : LocalizationTable
    {
        /// <summary>
        /// The key that will be used to add the table
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// The locales whose translations are contained in the table
        /// </summary>
        public List<Locale> Locales { get; set; }
    }
}
