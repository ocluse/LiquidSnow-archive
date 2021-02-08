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
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Thismaker.Goro"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Thismaker.Goro;assembly=Thismaker.Goro"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:GTextBox/>
    ///
    /// </summary>
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