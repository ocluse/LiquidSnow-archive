using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Thoth
{
    public class SideloadTable : LocalizationTable
    {
        public string Key { get; set; }
        public List<Locale> Locales { get; set; }
    }
}
