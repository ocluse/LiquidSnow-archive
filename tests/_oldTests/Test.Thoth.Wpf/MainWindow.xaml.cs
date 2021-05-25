using System.Windows;
using Thismaker.Thoth;

namespace Test.Thoth.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        public static async void Start()
        {
            await LocalizationManager.LoadData("localdata");
        }
    }
}
