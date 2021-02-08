using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro
{
    public class ColorSaturationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            var mode = "L";
            if (parameter != null)
            {
                mode = "D";

                if ((string)parameter == "W")
                {
                    mode = "W";
                }
            }

            var color = ((SolidColorBrush)value).Color;

            if (mode == "L")
            {
                color = ColorUtility.Lighten(color);
            }
            else if (mode == "W")
            {
                color = ColorUtility.Desaturate(color, 0.1f);
            }
            else
            {
                color = ColorUtility.Darken(color);
            }

            if (targetType == typeof(System.Windows.Media.Color)) return color;

            return ColorUtility.CreateBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

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
