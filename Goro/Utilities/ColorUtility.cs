using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Thismaker.Goro.Utilities
{
    public class ColorUtility
    {
        public static Color Parse(string hex)
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(hex);
            }
            catch
            {
                throw;
            }

        }

        public static bool TryParse(out Color c, string hex)
        {
            try
            {
                c = Parse(hex);
                return true;
            }
            catch
            {
                c = default;
                return false;
            }
        }

        public static SolidColorBrush CreateBrush(string hex)
        {
            return new SolidColorBrush(Parse(hex));
        }

        public static LinearGradientBrush CreateBrush(string hex1, string hex2)
        {
            return new LinearGradientBrush(Parse(hex1), Parse(hex2), 0);
        }
    }
}
