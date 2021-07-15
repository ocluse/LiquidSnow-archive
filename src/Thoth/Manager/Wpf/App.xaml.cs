using System.Windows;
using Thismaker.Goro;
using Thismaker.Goro.Utilities;

namespace Thismaker.Thoth.Manager.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeManager.Initialize();
            ThemeManager.Theme = ThemeMode.Dark;
            ThemeManager.Accent = ColorUtility.CreateBrush("#3A47AA").Color;

            MainWindow window = new();
            MainWindow = window;
            window.Show();
        }
    }
}
