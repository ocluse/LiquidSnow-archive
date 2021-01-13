
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace System.Windows
{
    public static class SystemWindows
    {
        public static Visibility ToVisibility(this bool state)
        {
            return state ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}

namespace System.Drawing
{
    public static class SystemDrawing
    {
        public static BitmapImage ToSource(this Bitmap src)
        {
            using var ms = new MemoryStream();
            src.Save(ms, ImageFormat.Jpeg);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
