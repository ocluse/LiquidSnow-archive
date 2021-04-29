using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Thismaker.Goro
{
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
        
        public Icon Icon
        {
            get { return (Icon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public IconDesign Design
        {
            get { return (IconDesign)GetValue(DesignProperty); }
            set { SetValue(DesignProperty, value); }
        }

        public SolidColorBrush Highlight
        {
            get { return (SolidColorBrush)GetValue(HighlightProperty); }
            set { SetValue(HighlightProperty, value); }
        }
    }
}