using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro
{
    public static class DefaultColors
    {
        static DefaultColors()
        {
            SpectreOriginal = ColorUtility.Parse("#7F47DD");
            Briliet = ColorUtility.Parse("#39B54A");
            LaSectur = ColorUtility.Parse("#00E2D1");
            Hibiscus = ColorUtility.Parse("#ED0F64");
            Oros = ColorUtility.Parse("#F4A800");
            ThismakerOfficial = ColorUtility.Parse("#0681FC");
        }

        /// <summary>
        /// The Official shade of Thismaker. It's a very mute blue
        /// </summary>
        public static Color ThismakerOfficial { get; private set; }
        
        /// <summary>
        /// The default color of the Spectre Series. Purplish.
        /// </summary>
        public static Color SpectreOriginal { get; private set; }
        
        /// <summary>
        /// The colors of Briliet, and Omondi, and is therefore green.
        /// </summary>
        public static Color Briliet { get; private set; }

        /// <summary>
        /// A pronounced troquise/cyan color of the LaSectur
        /// </summary>
        public static Color LaSectur { get; private set; }
        
        /// <summary>
        /// Warm and beutiful, a magenta color. It would smell noice too.
        /// </summary>
        public static Color Hibiscus { get; private set; }
        
        /// <summary>
        /// Gold. Gold. Gold.
        /// </summary>
        public static Color Oros { get; private set; }
    }

    /// <summary>
    /// A static class used to manage the overall theme of the <see cref="Goro"/> client
    /// </summary>
    public static class ThemeManager
    {

        #region Props and Fields


        private static ThemeMode _theme=ThemeMode.Light;

        private static Color _accent = DefaultColors.Briliet;
        
        /// <summary>
        /// Dark theme or light theme. Changing calls the <see cref="AccentChanged"/>
        /// event and therefore instantly changes the theme. The default value is <see cref="ThemeMode.Light"/>
        /// </summary>
        public static ThemeMode Theme
        {
            get { return _theme; }
            set
            {
                if (_theme == value) return;

                _theme = value;
                AccentChanged.Invoke(Accent);
            }
        }

        /// <summary>
        /// The accent color(topic) of the theme. Changing invokes the <see cref="AccentChanged"/>
        /// event and therefore instantly changes the accent. The default value is <see cref="DefaultColors.Briliet"/>
        /// </summary>
        public static Color Accent
        {
            get { return _accent; }
            set
            {
                if (_accent == value) return;

                _accent = value;
                AccentChanged?.Invoke(Accent);
            }
        }

        private static ResourceDictionary _dictionary;
        #endregion

        /// <summary>
        /// An event that is fired whenenver there is a change to the <see cref="Theme"/>/<see cref="Accent"/>.
        /// </summary>
        public static event Action<Color> AccentChanged;

        #region Private Methods

        //private static void MakeDefaultThemes()
        //{
        //    //Create the default themes:
        //    var spectre = ThemeBuilder.CreateTheme("Spectre Original", "#7F47DD", "#352260", "#FFC692FC", "#7F47DD", "#FFCA18E0");
        //    var briliet = ThemeBuilder.CreateTheme("Briliet Theme", "#39B54A", "#1C5421", "#9CE5A3", "#39B54A", "#299979");
        //    var sectur = ThemeBuilder.CreateTheme("The La Sectur", "#00E2D1", "#009381", "#71FFF1", "#FF86FFF8", "#FF00E26C");
        //    var magenta = ThemeBuilder.CreateTheme("Malvaceae Hibiscus", "#ED0F64", "#822347", "#FFFFA0C4", "#ED0F64", "#FF6B2A");
        //    var official = ThemeBuilder.CreateTheme("Thismaker Ofitsial'nyy", "#0681FC", "#1F5993", "#73B1EF", "#0681FC", "#FF0008A2");
        //    var oros = ThemeBuilder.CreateTheme("Mighty Oros", "#F4A800", "#7F5800", "#FFFFF4AD", "#F4A800", "#FFC0BC00");


        //    //Add the Themes:
        //    DefaultThemes = new List<Theme>
        //    {
        //        spectre,
        //        briliet,
        //        sectur,
        //        magenta,
        //        official,
        //        oros
        //    };
        //}

        //Styles that won't work properly
        private static void BindStyles()
        {
            var presentationFrameworkAssembly = typeof(Application).Assembly;
            var contextMenuStyle = _dictionary[typeof(ContextMenu)] as Style;
            var editorContextMenuType = Type.GetType("System.Windows.Documents.TextEditorContextMenu+EditorContextMenu, " + presentationFrameworkAssembly);

            if (editorContextMenuType != null)
            {
                var editorContextMenuStyle = new Style(editorContextMenuType, contextMenuStyle);
                _dictionary.Add(editorContextMenuType, editorContextMenuStyle);
            }

            var menuItemStyle = Application.Current.FindResource(typeof(MenuItem)) as Style;
            var editorMenuItemType = Type.GetType("System.Windows.Documents.TextEditorContextMenu+EditorMenuItem, " + presentationFrameworkAssembly);

            if (editorMenuItemType != null)
            {
                var editorContextMenuStyle = new Style(editorMenuItemType, menuItemStyle);
                Application.Current.Resources.Add(editorMenuItemType, editorContextMenuStyle);
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Initializes the ThemeManager class. 
        /// Must be called during the <see cref="Application.Startup"/> event.
        /// Failure to call this method may also lead to some controls, such as <see cref="ContextMenu"/>, not displaying correctly.
        /// </summary>
        public static void Initialize()
        {
            _dictionary = Application.Current.Resources;
            BindStyles();
            AccentChanged += OnAccentChanged;

            Accent = DefaultColors.Hibiscus;
            Theme = ThemeMode.Dark;
        }

        private static void OnAccentChanged(Color obj)
        {
            //Calculate the new accent dark and light:
            var accentLight = ColorUtility.CreateBrush(ColorUtility.Lighten(Accent));
            var accentDark = ColorUtility.CreateBrush(ColorUtility.Darken(Accent));
            var accentDisabled = ColorUtility.CreateBrush(ColorUtility.Desaturate(Accent, 0.1f));
            var accentShifted = ColorUtility.HueShift(Accent, 20);

            var bright = ColorUtility.CreateBrush("#ffffff");
            var dark = ColorUtility.CreateBrush("#1A1A1A");

            _dictionary["Accent"] = ColorUtility.CreateBrush(Accent);
            _dictionary["AccentLight"] = accentLight;
            _dictionary["AccentDark"] = accentDark;
            _dictionary["AccentDisabled"] = accentDisabled;
            _dictionary["AccentGradient"] = ColorUtility.CreateBrush(Accent, accentShifted);
            if (Theme == ThemeMode.Dark)
            {
                _dictionary["AccentDarkDynamic"] = accentDark;
                _dictionary["AccentLightDynamic"] = accentLight;
                _dictionary["PanelGray"] = ColorUtility.CreateBrush("#4D4D4D");
                _dictionary["PanelBackground"] = dark;
                _dictionary["PanelForeground"] = bright;

            }
            else
            {
                _dictionary["AccentDarkDynamic"] = accentLight;
                _dictionary["AccentLightDynamic"] = accentDark;
                _dictionary["PanelGray"] = ColorUtility.CreateBrush("#999999");
                _dictionary["PanelBackground"] = bright;
                _dictionary["PanelForeground"] = dark;

            }
        }
        #endregion

        
    }

    /// <summary>
    /// Represents either a dark or light theme
    /// </summary>
    public enum ThemeMode
    {
        /// <summary>
        /// Most of the background is white, while the foreground(e.g text) is black
        /// </summary>
        Light,
        
        /// <summary>
        /// Most of the background is dark, while the foreground(e.g text) is white
        /// </summary>
        Dark
    }
}
//QUESTIONS: Trust for advancement of Education, Trust for advancement of Religion