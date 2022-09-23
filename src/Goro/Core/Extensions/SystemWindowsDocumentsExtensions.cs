using System.Linq;
using System.Windows.Controls;

namespace System.Windows.Documents
{
    /// <summary>
    /// Extensions for System.Windows.Documents
    /// </summary>
    public static class SystemWindowsDocumentsExtensions
    {
        /// <summary>
        /// Determines if a <see cref="FlowDocument"/> is empty.
        /// </summary>
        public static bool IsEmpty(this FlowDocument document)
        {
            string text = new TextRange(document.ContentStart, document.ContentEnd).Text;
            if (string.IsNullOrWhiteSpace(text) == false)
                return false;
            else
            {
                if (document.Blocks.OfType<BlockUIContainer>()
                    .Select(c => c.Child).OfType<Image>()
                    .Any())
                    return false;
            }
            return true;
        }
    }
}