using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Thismaker.Goro.Utilities;
using Color = System.Windows.Media.Color;

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

            Color color;
            if (value.GetType() == typeof(SolidColorBrush))
            {
                color= ((SolidColorBrush)value).Color;
            }
            else if (value.ToString().StartsWith("#"))
            {
                color = ColorUtility.CreateBrush(value.ToString()).Color;
            }

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

    [ValueConversion(typeof(long), typeof(string))]
    public class FileSizeToReadableConverter : IValueConverter
    {
        public static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(long))
            {
                var size = (long)value;
                return GetBytesReadable(size);
            }
            else
            {
                return null;
            }    
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
