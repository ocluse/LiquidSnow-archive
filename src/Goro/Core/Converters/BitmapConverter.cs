using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro
{
    /// <summary>
    /// A converter that allows conversion of <see cref="Bitmap"/> to <see cref="BitmapImage"/>
    /// and vice-versa
    /// </summary>
    [ValueConversion(typeof(Bitmap), typeof(BitmapImage))]
    public class BitmapConverter : IValueConverter
    {
        /// <summary>
        /// Converts from <see cref="Bitmap"/> to <see cref="BitmapImage"/>
        /// </summary>
        /// <param name="value">The bitmap to convert</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }

            var image = (Bitmap)value;
            return BitmapUtility.Bitmap2BitmapImage(image);
        }

        /// <summary>
        /// Converts from <see cref="BitmapImage"/> to <see cref="Bitmap"/>
        /// </summary>
        /// <param name="value">The bitmapImage to convert</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">Ignored</param>
        /// <param name="culture">Ignored</param>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)         
            {
                return null; 
            }

            var image = (BitmapImage)value;
            return BitmapUtility.BitmapImage2Bitmap(image);
        }
    }
}