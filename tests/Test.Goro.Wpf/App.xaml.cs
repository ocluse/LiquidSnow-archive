using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
