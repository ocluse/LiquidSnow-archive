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

        public static SolidColorBrush CreateBrush(Color c)
        {
            return new SolidColorBrush(c);
        }

        public static SolidColorBrush CreateBrush(string hex)
        {
            return CreateBrush(Parse(hex));
        }

        public static LinearGradientBrush CreateBrush(Color c1, Color c2)
        {
            return new LinearGradientBrush(c1, c2, 0);
        }

        public static LinearGradientBrush CreateBrush(string hex1, string hex2)
        {
            return CreateBrush(Parse(hex1), Parse(hex2));
        }

        public static Color Darken(Color color)
        {
            var hls = new HSLColor(color);
            return hls.Darker(0.1f);
        }

        public static Color Lighten(Color color)
        {
            var hls = new HSLColor(color);
            return hls.Lighter(1f);
        }

        public static Color Desaturate(Color color, float percent=0.5f)
        {
            var hsl = new HSLColor(color);

            var newSat = (int)(hsl.Saturation * percent);

            return HSLColor.ColorFromHSL(hsl.Hue, newSat, hsl.Luminosity);
        }

        public static Color HueShift(Color color, int amount)
        {
            var hls = new HSLColor(color);
            var newHue = hls.Hue + amount;
            
            while (newHue > HSLColor.HSLMax)
            {
                newHue -= HSLColor.HSLMax;
            }

            return HSLColor.ColorFromHSL(newHue, hls.Saturation, hls.Luminosity);
        }
    }
}
