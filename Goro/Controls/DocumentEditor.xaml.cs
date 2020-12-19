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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Media = System.Windows.Media;
using System.Linq;

namespace Thismaker.Goro
{
    /// <summary>
    /// Interaction logic for DocumentEditor.xaml
    /// </summary>
    public partial class DocumentEditor : UserControl
    {
        ListCollectionView Fonts { get; set; }
        public DocumentEditor()
        {
            InitializeComponent();

            Fonts = CollectionViewSource.GetDefaultView(Media.Fonts.SystemFontFamilies) as ListCollectionView;

            ComboFonts.ItemsSource = Fonts;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void ComboFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var font = (FontFamily)ComboFonts.SelectedItem;
            if (Doc.Selection.IsEmpty) Doc.FontFamily = font;
        }
    }
}
