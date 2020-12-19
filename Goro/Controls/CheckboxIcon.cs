using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Thismaker.Goro
{
    public class CheckboxIcon : CheckBox
    {
        static CheckboxIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CheckboxIcon), new FrameworkPropertyMetadata(typeof(CheckboxIcon)));
        }

        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register("Icon", typeof(IconType), typeof(CheckboxIcon), new PropertyMetadata(IconType.GlobalNavigationButton));

        public IconType Icon
        {
            get { return (IconType)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
    }
}
