using System.Windows;
using Thismaker.Goro;

namespace Test.Goro.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeManager.Initialize();
            var main = new MainWindow();
            MainWindow = main;
            main.Show();
        }
    }
}
