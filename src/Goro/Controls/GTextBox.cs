using System.Windows;
using System.Windows.Controls;

namespace Thismaker.Goro
{
    public class GTextBox : TextBox
    {
        static GTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GTextBox), new FrameworkPropertyMetadata(typeof(GTextBox)));
        }

        public static readonly DependencyProperty PlaceholderTextProperty
            = DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(GTextBox), new PropertyMetadata());

        public static readonly DependencyProperty HeaderProperty
           = DependencyProperty.Register(nameof(Header), typeof(string), typeof(GTextBox), new PropertyMetadata());

        public string PlaceholderText
        {
            get { return (string)GetValue(PlaceholderTextProperty); }
            set { SetValue(PlaceholderTextProperty, value); }
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}