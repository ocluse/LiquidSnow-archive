using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Thismaker.Goro;

namespace Test.Goro.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            rnd = new Random();

            Accents.Add(DefaultColors.Briliet);
            Accents.Add(DefaultColors.Hibiscus);
            Accents.Add(DefaultColors.LaSectur);
            Accents.Add(DefaultColors.Oros);
            Accents.Add(DefaultColors.SpectreOriginal);
            Accents.Add(DefaultColors.ThismakerOfficial);
            ApplyRandomAccent();
        }

        List<Color> Accents = new List<Color>();
        Random rnd;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Theme = ThemeManager.Theme == ThemeMode.Dark ? ThemeMode.Light : ThemeMode.Dark;

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplyRandomAccent();   
        }

        public void ApplyRandomAccent()
        {
            var rand = rnd.Next(0, Accents.Count);
            ThemeManager.Accent = Accents[rand];
        }
    }
}
