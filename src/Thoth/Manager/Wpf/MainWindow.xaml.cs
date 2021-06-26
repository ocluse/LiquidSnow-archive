using System;
using System.Windows;

namespace Thismaker.Thoth.Manager.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;

        private Func<bool> _contextChangeChecker;
        public MainWindow()
        {
            _instance = this;
            InitializeComponent();
            LocaleConverter.Model = (MainViewModel)DataContext;
        }

        public static void SetContextChangeChecker(Func<bool> function)
        {
            _instance._contextChangeChecker = function;
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_contextChangeChecker == null)
            {
                return;
            }

            e.Cancel = !_contextChangeChecker.Invoke();

        }
    }
}
