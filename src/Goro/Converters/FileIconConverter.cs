using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using Thismaker.Goro.Utilities;

namespace Thismaker.Goro.Converters
{
    /// <summary>
    /// Returns the appropriate icon depending on the current enviroment based on the file name/extension e.g "baba.txt".
    /// The return value is always an <see cref="ImageSource"/> that can be plugged into a WPF image. This converter is one-way.
    /// </summary>
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class FileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return FileIconUtility.FindIconForFilename("test", true);

            var name = (string)value;

            return FileIconUtility.FindIconForFilename(name, true);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
//#IT COULD BE HERE