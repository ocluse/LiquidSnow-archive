using System;
using System.Globalization;
using System.Windows.Data;
using Thismaker.Core.Utilities;

namespace Thismaker.Goro
{
    /// <summary>
    /// Converts a <see cref="long"/> value representing file size in bytes to
    /// a more readable string, for example 2MB or 34.5GB
    /// </summary>
    [ValueConversion(typeof(long), typeof(string))]
    public class FileSizeToReadableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var size = (long)value;
            return IOUtility.GetFileSizeReadable(size);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}