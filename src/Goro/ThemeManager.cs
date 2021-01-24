using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Thismaker.Goro
{
    public static class ThemeManager
    {
        public static List<Theme> DefaultThemes { get; private set; }

        static ResourceDictionary context;

        static ThemeManager()
        {
            //Create the default themes:
            var spectre = ThemeBuilder.CreateTheme("Spectre Original", "#7F47DD", "#352260", "#FFC692FC", "#7F47DD", "#FFCA18E0");
            var briliet = ThemeBuilder.CreateTheme("Briliet Theme", "#39B54A", "#1C5421", "#9CE5A3", "#39B54A", "#299979");
            var sectur = ThemeBuilder.CreateTheme("The La Sectur", "#00E2D1", "#009381", "#71FFF1", "#FF86FFF8", "#FF00E26C");
            var magenta = ThemeBuilder.CreateTheme("Malvaceae Hibiscus", "#ED0F64", "#822347", "#FFFFA0C4", "#ED0F64", "#FF6B2A");
            var official = ThemeBuilder.CreateTheme("Thismaker Ofitsial'nyy", "#0681FC", "#1F5993", "#73B1EF", "#0681FC", "#FF0008A2");
            var oros = ThemeBuilder.CreateTheme("Mighty Oros", "#F4A800", "#7F5800", "#FFFFF4AD", "#F4A800", "#FFC0BC00");


            //Add the Themes:
            DefaultThemes = new List<Theme>
            {
                spectre,
                briliet,
                sectur,
                magenta,
                official,
                oros
            };
        }

        public static void Initialize(ResourceDictionary dictionary)
        {
            context = dictionary;
        }

        public static void SetTheme(DefaultTheme defaultTheme)
        {
            int index = (int)defaultTheme;

            var theme = DefaultThemes[index];
            SetTheme(theme);
        }

        public static void SetTheme(Theme theme)
        {
            context["Accent"] = theme.Accent;
            context["AccentDark"] = theme.AccentDark;
            context["AccentLight"] = theme.AccentLight;
            context["AccentTransparent"] = theme.AccentTransparent;
            context["AccentGradient"] = theme.AccentGradient;
        }
    }

    public enum DefaultTheme
    {
        SpectreOriginal,
        BrillietTheme,
        LaSectur,
        Hibiscus,
        ThismakerOfficial,
        MightyOros,
    }
}
//QUESTIONS: Trust for advancement of Education, Trust for advancement of Religion