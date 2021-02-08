using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Thismaker.Goro
{
    /// <summary>
    /// A static class used to load packed icons and resources
    /// </summary>
    public static partial class IconManager
    {
        /// <summary>
        /// Loads the icon XamlData into a drawing group
        /// </summary>
        /// <param name="icon">The icon to load</param>
        /// <param name="scheme">The scheme to load. Cannot load <see cref="IconScheme.Segoe"/> as that is a font</param>
        /// <returns>An image, represented by a drawing group, of the icon</returns>
        public static DrawingGroup LoadIcon(Icon icon, IconDesign scheme)
        {
            var schemeName = scheme switch
            {
                IconDesign.ArethaDesign => "Aretha",
                IconDesign.MaterialDesign => "Material",
                _ => throw new InvalidOperationException("Invalid scheme")
            };

            var assembly = typeof(IconManager).Assembly;

            using Stream stream = assembly.GetManifestResourceStream($"Thismaker.Goro.Assets.Icons.{schemeName}.{icon}.xaml");
            if (stream == null) throw new NullReferenceException("Icon not found");

            return (DrawingGroup)XamlReader.Load(stream);
        }

        /// <summary>
        /// Loads the icon as a brush, that can be used as an opacity mask
        /// </summary>
        /// <param name="icon">The icon to load</param>
        /// <param name="scheme">The scheme to use. Cannot load <see cref="IconScheme.Segoe"/> as that is a font</param>
        /// <returns>A brush that comprises the icon</returns>
        public static DrawingBrush LoadIconAsBrush(Icon icon, IconDesign scheme)
        {
            try
            {
                var drawing = LoadIcon(icon, scheme);
                var brush = new DrawingBrush(drawing)
                {
                    AlignmentX = AlignmentX.Center,
                    AlignmentY = AlignmentY.Center,
                    Stretch = Stretch.Uniform
                };
                return brush;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads icon as an image that can be plugged into <see cref="Image.Source"/>
        /// </summary>
        /// <param name="icon">The icon to load</param>
        /// <param name="scheme">The scheme to use. Cannot load <see cref="IconScheme.Segoe"/> as that is a fon</param>
        /// <returns>An image that can be used as the source of a WPF <see cref="Image"/> control</returns>
        public static DrawingImage LoadIconAsImage(Icon icon, IconDesign scheme)
        {
            try
            {
                var drawing = LoadIcon(icon, scheme);
                return new DrawingImage(drawing);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads the icon then converts it to a <see cref="BitmapSource"/>
        /// This is highly unrecommended, as of now.
        /// </summary>
        /// <param name="icon">The icon to load</param>
        /// <param name="scheme">The scheme to use.</param>
        /// <returns><see cref="BitmapSource"/> containing the information on the icon</returns>
        public static BitmapSource LoadIconAsBitmap(Icon icon, IconDesign scheme)
        {
            try
            {
                var drawingImage = LoadIconAsImage(icon, scheme);
                var image = new Image { Source = drawingImage };

                var bitmap = new RenderTargetBitmap((int)drawingImage.Width, (int)drawingImage.Height, 96, 96, PixelFormats.Pbgra32);
                image.Arrange(new System.Windows.Rect(0, 0, bitmap.Width, bitmap.Height));
                bitmap.Render(image);

                return bitmap;
            }
            catch
            {
                throw;
            }
        }
    }
}