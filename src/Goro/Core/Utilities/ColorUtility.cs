using System.Windows.Media;

namespace Thismaker.Goro.Utilities
{
    /// <summary>
    /// Contains utility methods for Parsing hex-strings to colors, manipulatig colors and creating brushes.
    /// </summary>
    public class ColorUtility
    {
        /// <summary>
        /// Converts a hexadecimal number into the color it represents, e.g #ffffff for white.
        /// </summary>
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

        /// <summary>
        /// Trys to convet a hexadecimal string to a color, if successful returns the color and true,
        /// otherwise false.
        /// </summary>
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

        /// <summary>
        /// Returns a <see cref="SolidColorBrush"/> from the color
        /// </summary>
        public static SolidColorBrush CreateBrush(Color c)
        {
            return new SolidColorBrush(c);
        }

        /// <summary>
        /// Returns a <see cref="SolidColorBrush"/> from the hexadecimal string
        /// </summary>
        public static SolidColorBrush CreateBrush(string hex)
        {
            return CreateBrush(Parse(hex));
        }

        /// <summary>
        /// Creates a <see cref="LinearGradientBrush"/> from the provided colors.
        /// </summary>
        public static LinearGradientBrush CreateBrush(Color c1, Color c2)
        {
            return new LinearGradientBrush(c1, c2, 0);
        }

        /// <summary>
        /// Creats a <see cref="LinearGradientBrush"/> from the provided hexadecimal strings
        /// </summary>
        public static LinearGradientBrush CreateBrush(string hex1, string hex2)
        {
            return CreateBrush(Parse(hex1), Parse(hex2));
        }

        /// <summary>
        /// Darkens a color.
        /// </summary>
        public static Color Darken(Color color)
        {
            var hls = new HSLColor(color);
            return hls.Darker(0.1f);
        }

        /// <summary>
        /// Lightens a color, returning a more pale version
        /// </summary>
        public static Color Lighten(Color color)
        {
            var hls = new HSLColor(color);
            return hls.Lighter(1f);
        }

        /// <summary>
        /// Removes saturation from a color, turning it more grayer.
        /// </summary>
        public static Color Desaturate(Color color, float percent=0.5f)
        {
            var hsl = new HSLColor(color);

            var newSat = (int)(hsl.Saturation * percent);

            return HSLColor.ColorFromHSL(hsl.Hue, newSat, hsl.Luminosity);
        }

        /// <summary>
        /// Changes the hue of a color by the provided amount.
        /// </summary>
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
