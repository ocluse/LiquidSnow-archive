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
using System.Windows.Shapes;

namespace Thismaker.Goro
{
    /// <summary>
    /// Interaction logic for GoroMessageBox.xaml
    /// </summary>
    public partial class GoroMessageBox : Window
    {
        public static IconDesign Design { get; set; }

        public static void Show(string content)
        {
            
            Show("", content, MessageBoxButton.OK);
        }

        public static void Show(string title, string content)
        {
            Show(title, content, MessageBoxButton.OK);
        }

        public static bool? Show(string title, string content, MessageBoxButton button, StatusInfo? status=null)
        {
            if (button == MessageBoxButton.YesNoCancel) throw new InvalidOperationException("MessageBoxButton.YesNoCancel is currently unsupported");

            var dlg = new GoroMessageBox();

            dlg.Button2.Visibility = button == MessageBoxButton.OK ? Visibility.Collapsed : Visibility.Visible;

            dlg.Title = title;
            dlg.ContentText.Text = content;
            dlg.Button1.Content = button == MessageBoxButton.YesNo ? "Yes" : "OK";
            dlg.Button2.Content = button == MessageBoxButton.YesNo ? "No" : "Cancel";

            if (status == null)
            {
                dlg.Status.Visibility = Visibility.Collapsed;

            }
            else
            {
                dlg.Status.Visibility = Visibility.Visible;
                dlg.Status.Design = Design;
                dlg.Status.Status = status.Value;
            }

            return dlg.ShowDialog();
        }

        internal GoroMessageBox()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
