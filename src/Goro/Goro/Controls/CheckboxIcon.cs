using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Thismaker.Goro
{
    /// <summary>
    /// A <see cref="CheckBox"/> that shows up as an icon
    /// </summary>
    public class CheckboxIcon : CheckBox
    {
        static CheckboxIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckboxIcon), new FrameworkPropertyMetadata(typeof(CheckboxIcon)));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(CheckboxIcon), new PropertyMetadata(Icon.None));
        public static readonly DependencyProperty DesignProperty
            = DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(CheckboxIcon), new PropertyMetadata(IconDesign.Segoe));
        public static readonly DependencyProperty HighlightProperty
            = DependencyProperty.Register(nameof(Highlight), typeof(SolidColorBrush), typeof(CheckboxIcon));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// The icon of the button
        /// </summary>
        public Icon Icon
        {
            get => (Icon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// The design of the button's icon
        /// </summary>
        public IconDesign Design
        {
            get => (IconDesign)GetValue(DesignProperty);
            set => SetValue(DesignProperty, value);
        }

        /// <summary>
        /// The highlight of the button when shown
        /// </summary>
        public SolidColorBrush Highlight
        {
            get => (SolidColorBrush)GetValue(HighlightProperty);
            set => SetValue(HighlightProperty, value);
        }
    }
}