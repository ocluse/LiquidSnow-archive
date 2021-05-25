using System.Linq;
using System.Windows.Controls;

namespace System.Windows.Documents
{
    public static class SystemDocuments
    {
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