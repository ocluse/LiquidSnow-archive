using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Pluto
{
    public static class Pluto
    {
        public static AccountType GetTypeName(string type)
        {
            if (int.TryParse(type, out int x))
            {
                return (AccountType)x;
            }
            return AccountType.Asset;
        }

        public static string GetTypeVal(AccountType type)
        {
            return ((int)type).ToString();
        }
    }
}
