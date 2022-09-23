using System.Windows;
using System.Windows.Controls;

namespace Thismaker.Goro
{
    /// <summary>
    /// A WPF control for displaying simple icon or symbol from the various design collections e.g the Segoe MDL2 Assets
    /// </summary>
    public class SymbolIcon : Control
    {
        static SymbolIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SymbolIcon), new FrameworkPropertyMetadata(typeof(SymbolIcon)));
        }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(SymbolIcon), new PropertyMetadata(Icon.None));
        public static readonly DependencyProperty DesignProperty
            = DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(SymbolIcon), new PropertyMetadata(IconDesign.Segoe));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <summary>
        /// The Icon to be displayed by the <see cref="SymbolIcon"/>
        /// </summary>
        public Icon Icon
        {
            get => (Icon)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// The Design of the icon displayed
        /// </summary>
        public IconDesign Design
        {
            get => (IconDesign)GetValue(DesignProperty);
            set => SetValue(IconProperty, value);
        }
    }
}