using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;

namespace Thismaker.Goro
{
    /// <summary>
    /// Interaction logic for DocumentEditor.xaml
    /// </summary>
    public partial class DocumentEditor : UserControl
    {
        public DocumentEditor()
        {
            InitializeComponent();
            

            try
            {
                ((DocumentEditorViewModel)DataContext).Attach(Doc);
                Design = ThemeManager.DefaultDesign;
            }
            catch
            {

            }
            
        }

        public IconDesign Design
        {
            get { return (IconDesign)this.GetValue(DesignProperty); }
            set { SetValue(DesignProperty, value); }
        }

        public static readonly DependencyProperty DesignProperty =
            DependencyProperty.Register(nameof(Design), typeof(IconDesign), typeof(DocumentEditor));

        public void Save(Stream stream)
        {
            try
            {
                var range = new TextRange(Doc.Document.ContentStart, Doc.Document.ContentEnd);
                range.Save(stream, DataFormats.Rtf);
            }
            catch
            {
                throw;
            }
        }

        public void Load(Stream stream)
        {
            try
            {
                var range = new TextRange(Doc.Document.ContentStart, Doc.Document.ContentEnd);
                range.Load(stream, DataFormats.Rtf);
            }
            catch
            {
                throw;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
