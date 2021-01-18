using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro
{
    [ValueConversion(typeof(Bitmap), typeof(BitmapImage))]
    public class BitmapConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var image = (Bitmap)value;
            return BitmapUtility.BitmapToBitmapImage(image);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(DateTime),typeof(string))]
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var date = ((DateTime)value).ToLocalTime();

            var dif = DateTime.Today - date;
            var days = dif.TotalDays;

            if (days < 1 && DateTime.Today <= date)
            {
                return $"Today, {date:hh:mm tt}";
            }
            if (days < 7)
            {
                var prefix = date < DateTime.Today ? "Last" : "Next";
                return $"{prefix} {date:dddd, hh:mm tt}";
            }

            return date.ToString("dddd, MMM M, yyyy, hh:mm tt");

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
