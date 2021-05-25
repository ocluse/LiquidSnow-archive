using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Thismaker.Goro
{
    /// <summary>
    /// Allows the conversion from <see cref="bool"/> to <see cref="Visibility"/> and vice-versa.
    /// This converter accepts a parameter string "inverse" that returns the opposite value.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (bool)value;

            bool inverse = parameter != null && parameter.GetType() == typeof(string) && parameter.ToString().ToLower() == "inverse";

            return inverse ? (!state).ToVisibility() : state.ToVisibility();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (Visibility)value;

            bool inverse = parameter != null && parameter.GetType() == typeof(string) && parameter.ToString().ToLower() == "inverse";

            var vals = state == Visibility.Visible;

            return inverse ? !vals : vals;
        }
    }
}