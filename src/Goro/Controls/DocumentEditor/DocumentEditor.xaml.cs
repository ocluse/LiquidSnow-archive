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
            ((DocumentEditorViewModel)DataContext).Attach(Doc);
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
    }
}
