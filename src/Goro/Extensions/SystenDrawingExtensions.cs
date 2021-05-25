using System.Windows.Media.Imaging;
using Thismaker.Goro.Utilities;

namespace System.Drawing
{
    public static class SystemDrawing
    {
        public static BitmapImage ToSource(this Bitmap src)
        {
            return BitmapUtility.Bitmap2BitmapImage(src);
        }
    }
}