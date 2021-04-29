using System.Windows;
using System.Windows.Controls;

namespace Thismaker.Goro
{
    public class SymbolIcon : Control
    {
        static SymbolIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SymbolIcon), new FrameworkPropertyMetadata(typeof(SymbolIcon)));
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(SymbolIcon), new PropertyMetadata(Icon.None));
        public static readonly DependencyProperty DesignProperty
            = DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(SymbolIcon), new PropertyMetadata(IconDesign.Segoe));

        public Icon Icon
        {
            get { return (Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public IconDesign Design
        {
            get { return (IconDesign)GetValue(DesignProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}