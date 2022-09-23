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
        /// <summary>
        /// Creates a new instance of the <see cref="DocumentEditor"/>
        /// </summary>
        public DocumentEditor()
        {
            InitializeComponent();
            
            try
            {
                ((DocumentEditorViewModel)DataContext).Attach(Doc);
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// Saves the contents of the editor to the provided stream
        /// </summary>
        /// <param name="stream">The stream to save to</param>
        /// <param name="format">If provided, the format that the document is saved using, otherwise RTF</param>
        public void Save(Stream stream, string? format=null)
        {
            if (format == null)
            {
                format = DataFormats.Rtf;
            }
            var range = new TextRange(Doc.Document.ContentStart, Doc.Document.ContentEnd);
            range.Save(stream, format);
        }

        /// <summary>
        /// Loads data to the editor from the provided stream
        /// </summary>
        /// <param name="stream">The stream to load from</param>
        /// <param name="format">If provided, the format to load the data using, otherwise RTF</param>
        public void Load(Stream stream, string? format=null)
        {
            if(format == null)
            {
                format= DataFormats.Rtf;
            }

            var range = new TextRange(Doc.Document.ContentStart, Doc.Document.ContentEnd);
            range.Load(stream, format);
        }
    }
}
