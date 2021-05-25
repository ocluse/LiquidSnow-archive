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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var image = (Bitmap)value;
            return BitmapUtility.Bitmap2BitmapImage(image);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var image = (BitmapImage)value;
            return BitmapUtility.BitmapImage2Bitmap(image);
        }
    }
}