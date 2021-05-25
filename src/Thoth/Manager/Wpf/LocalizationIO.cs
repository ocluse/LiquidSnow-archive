using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thismaker.Thoth.Manager.Wpf
{
    class LocalizationIO : LocalizationIOBase
    {
        protected override LocalizationData Deserialize(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<LocalizationData>(json);
        }

        protected override string Serialize(LocalizationData data)
        {
            return System.Text.Json.JsonSerializer.Serialize(data);
        }
    }
}
