using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Thismaker.Goro.Utilities
{
    /// <summary>
    /// Contains utility methods for converting <see cref="Bitmap"/> to <see cref="BitmapSource"/> and vice versa
    /// </summary>
    public static class BitmapUtility
    {
        /// <summary>
        /// Converts a <see cref="Bitmap"/> to <see cref="BitmapImage"/>
        /// </summary>
        public static BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            var ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            var image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();

            return image;
        }

        /// <summary>
        /// Converts a <see cref="BitmapImage"/> to a <see cref="Bitmap"/>
        /// </summary>
        public static Bitmap BitmapImage2Bitmap(BitmapImage image)
        {
            using var msOut = new MemoryStream();
            var enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));
            enc.Save(msOut);
            var bitmap = Image.FromStream(msOut);

            return new Bitmap(bitmap);
        }
    }
}