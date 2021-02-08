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
            = DependencyProperty.Register(nameof(Icon), typeof(Icon), typeof(CheckboxIcon), new PropertyMetadata(Icon.None));
        public static readonly DependencyProperty DesignProperty
            = DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(CheckboxIcon), new PropertyMetadata(IconDesign.Segoe));

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
    }
}