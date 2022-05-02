using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Thismaker.Goro;

namespace Thismaker.Thoth.Manager.Wpf
{
    /// <summary>
    /// Interaction logic for GenerateWindow.xaml
    /// </summary>
    public partial class GenerateWindow : Window
    {
        private readonly LocalizationData _locData;
        
        public GenerateWindow(LocalizationData data)
        {
            InitializeComponent();
            _locData = data;
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            string outDir=txtOutputDir.Text;
            string ns = txtNamespace.Text;
            
            if (string.IsNullOrEmpty(ns))
            {
                GoroMessageBox.Show("Error", "Please provide a namespace value", MessageBoxButton.OK, StatusInfo.Error);
                return;
            }

            if (string.IsNullOrEmpty(outDir))
            {
                GoroMessageBox.Show("Error", "Please provide an output directory", MessageBoxButton.OK, StatusInfo.Error);
                return;
            }

            if (!System.IO.Directory.Exists(outDir))
            {
                var result = GoroMessageBox.Show("Create Directory?", $"The  directory '{outDir}' does not exist. Do you wish to create it?", MessageBoxButton.YesNo, StatusInfo.Warning);

                if(result.HasValue && result.Value)
                {
                    System.IO.Directory.CreateDirectory(outDir);
                }
                else
                {
                    return;
                }
            }

            TemplateGenerator.GenerateTemplate(_locData, outDir, ns);
            Close();
        }

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dlg = new();
            var result = dlg.ShowDialog();

            if(result.HasValue && result.Value)
            {
                txtOutputDir.Text = dlg.FileName;
            }
        }
    }
}
