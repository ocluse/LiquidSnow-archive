using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Thismaker.Goro
{
    /// <summary>
    /// A <see cref="Button"/> that shows up as an icon instead
    /// </summary>
    public class ButtonIcon : Button
    {
        static ButtonIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonIcon), new FrameworkPropertyMetadata(typeof(ButtonIcon)));
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(ButtonIcon), new PropertyMetadata(Icon.None));
        public static readonly DependencyProperty DesignProperty
            = DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(ButtonIcon), new PropertyMetadata(IconDesign.Segoe));
        public static readonly DependencyProperty HighlightProperty
            = DependencyProperty.Register(nameof(Highlight), typeof(SolidColorBrush), typeof(ButtonIcon));
        
        /// <summary>
        /// The icon of the button
        /// </summary>
        public Icon Icon
        {
            get { return (Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// The design of the button's icon
        /// </summary>
        public IconDesign Design
        {
            get { return (IconDesign)GetValue(DesignProperty); }
            set { SetValue(DesignProperty, value); }
        }

        /// <summary>
        /// The highlight of the button when shown
        /// </summary>
        public SolidColorBrush Highlight
        {
            get { return (SolidColorBrush)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }
    }
}