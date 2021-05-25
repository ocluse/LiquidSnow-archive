using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
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
        readonly Random rnd;

        private void ShowMsgBox_Click(object sender, RoutedEventArgs e)
        {
            var result = GoroMessageBox.Show("Hello", "Where is my mother? Lorem ipsum sint amen faxi daremones", MessageBoxButton.YesNoCancel, "Do you want to do this another time?", out bool? optional);
            GoroMessageBox.Show(optional.ToString());
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            ThemeManager.Theme = ThemeManager.Theme == ThemeMode.Dark ? ThemeMode.Light : ThemeMode.Dark;
        }

        private void ChangeAccent_Click(object sender, RoutedEventArgs e)
        {
            ApplyRandomAccent();
        }

        public void ApplyRandomAccent()
        {
            var rand = rnd.Next(0, Accents.Count);
            ThemeManager.Accent = Accents[rand];
        }

        private void ChangeIconDesign_Click(object sender, RoutedEventArgs e)
        {
            if (ThemeManager.DefaultDesign == IconDesign.Segoe)
            {
                ThemeManager.DefaultDesign = IconDesign.MaterialDesign;
            }
            else
            {
                ThemeManager.DefaultDesign = IconDesign.Segoe;
            }
        }
    }
}
