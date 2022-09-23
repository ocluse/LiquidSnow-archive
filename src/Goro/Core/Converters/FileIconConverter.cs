using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro.Converters
{
    /// <summary>
    /// Returns the appropriate icon depending on the current enviroment based on the file name/extension e.g "baba.txt".
    /// </summary>
    /// <remarks>
    /// The return value is always an <see cref="ImageSource"/> that can be plugged into a WPF image.
    /// </remarks>
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class FileIconConverter : IValueConverter
    {
        /// <summary>
        /// Converts a filename or extension to the appropriate icon, returned as a <see cref="ImageSource"/>.
        /// </summary>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null)
            {
                return null;
            }
            else if(value is string name)
            {
                return FileIconUtility.GetIcon(name, true);
            }
            else
            {
                throw new ArgumentException("Value provided must be a string", nameof(value));
            }            
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
//#IT COULD BE HERE