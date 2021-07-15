using System.Windows;

namespace Thismaker.Thoth.Manager.Wpf
{
    /// <summary>
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow(string header, string input = null)
        {
            InitializeComponent();
            ((InputWindowModel)DataContext).Header = header;
            ((InputWindowModel)DataContext).InputString = input;
        }

        public string Input
        {
            get
            {
                return ((InputWindowModel)DataContext).InputString;
            }
        }
    }
}
