using System;
using System.Windows.Media;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro
{
    public class Theme
    {
        /// <summary>
        /// The name identifier of the theme
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The accent is the main color of the theme
        /// </summary>
        public SolidColorBrush Accent { get; set; }

        /// <summary>
        /// The dark version of the theme's main color
        /// </summary>
        public SolidColorBrush AccentDark { get; set; }

        /// <summary>
        /// The light version of the theme's main color
        /// </summary>
        public SolidColorBrush AccentLight { get; set; }

        /// <summary>
        /// The transparent version of the theme's main color, a transparency of 0.4 is applied
        /// </summary>
        public SolidColorBrush AccentTransparent { get; set; }

        /// <summary>
        /// The gradient version of the theme
        /// </summary>
        public LinearGradientBrush AccentGradient { get; set; }
    }


    public static class ThemeBuilder
    {
        public static Theme CreateTheme(string name, string accent, string accentDark, string accentLight, string accentTrans, string accentGrad)
        {
            var theme = new Theme
            {
                Accent = ColorUtility.CreateBrush(accent),
                AccentDark = ColorUtility.CreateBrush(accentDark),
                AccentLight = ColorUtility.CreateBrush(accentLight),
                AccentTransparent = ColorUtility.CreateBrush(accentTrans),
                AccentGradient = ColorUtility.CreateBrush(accent, accentGrad)
            };

            theme.AccentTransparent.Opacity = 0.4;

            return theme;
        }
    }
}
