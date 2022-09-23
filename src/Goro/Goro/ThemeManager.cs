using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro
{
    /// <summary>
    /// A static class used to manage the overall theme of the Goro application
    /// </summary>
    public static class ThemeManager
    {

        #region Props and Fields

        private static ThemeMode _theme = ThemeMode.Light;

        private static Color _accent = DefaultColors.Briliet;

        /// <summary>
        /// Dark theme or light theme. Changing calls the <see cref="AccentChanged"/>
        /// event and therefore instantly changes the theme. The default value is <see cref="ThemeMode.Light"/>
        /// </summary>
        public static ThemeMode Theme
        {
            get => _theme;
            set
            {
                if (_theme == value) return;

                _theme = value;
                AccentChanged?.Invoke(Accent);
            }
        }

        /// <summary>
        /// The accent color(topic) of the theme. Changing invokes the <see cref="AccentChanged"/>
        /// event and therefore instantly changes the accent. The default value is <see cref="DefaultColors.Briliet"/>
        /// </summary>
        public static Color Accent
        {
            get => _accent;
            set
            {
                if (_accent == value) return;

                _accent = value;
                AccentChanged?.Invoke(Accent);
            }
        }

        /// <summary>
        /// The default design applied to Icons in the entire application
        /// </summary>
        public static IconDesign DefaultDesign
        {
            get
            {
                if(_dictionary is null)
                {
                    throw new InvalidOperationException("Goro has not yet been initialized");
                }

                return (IconDesign)_dictionary["DefaultDesign"];
            }
            set
            {
                if (_dictionary is null)
                {
                    throw new InvalidOperationException("Goro has not yet been initialized");
                }

                _dictionary["DefaultDesign"] = value;
            }
        }

        private static ResourceDictionary? _dictionary;
        #endregion

        /// <summary>
        /// An event that is fired whenenver there is a change to the <see cref="Theme"/>/<see cref="Accent"/>.
        /// </summary>
        public static event Action<Color>? AccentChanged;

        #region Private Methods

        //Styles that won't work properly
        private static void BindStyles()
        {
            if(_dictionary is null)
            {
                throw new InvalidOperationException("Goro has not yet been initialized");
            }

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
            if (_dictionary is null)
            {
                throw new InvalidOperationException("Goro has not yet been initialized");
            }

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
                _dictionary["PanelGrayResponsive"] = ColorUtility.CreateBrush("#303030");
            }
            else
            {
                _dictionary["AccentDarkDynamic"] = accentLight;
                _dictionary["AccentLightDynamic"] = accentDark;
                _dictionary["PanelGray"] = ColorUtility.CreateBrush("#999999");
                _dictionary["PanelBackground"] = bright;
                _dictionary["PanelForeground"] = dark;
                _dictionary["PanelGrayResponsive"] = ColorUtility.CreateBrush("#F2F2F2");
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