using System;
using System.Globalization;
using System.Windows.Data;
using Thismaker.Core.Utilities;

namespace Thismaker.Goro
{
    /// <summary>
    /// Converts a <see cref="long"/> value representing file size in bytes to a more readable string, for example 2MB or 34.5GB.
    /// </summary>
    [ValueConversion(typeof(long), typeof(string))]
    public class FileSizeToReadableConverter : IValueConverter
    {
        /// <summary>
        /// Converts size in bytes represented as long to a human readable string.
        /// </summary>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null)
            {
                return null;
            }

            if(value is long size)
            {
                return IOUtility.GetFileSizeReadable(size);
            }
            else
            {
                throw new ArgumentException("Value provided must be a long", nameof(value));
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}