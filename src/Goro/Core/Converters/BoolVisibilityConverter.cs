using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Thismaker.Goro
{
    /// <summary>
    /// A converter that converts from <see cref="bool"/> to <see cref="Visibility"/> and vice-versa.
    /// </summary>
    /// <remarks>
    /// This converter accepts a parameter string "inverse" that returns the opposite value.
    /// </remarks>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts from a bool to an <see cref="Visibility"/> value.
        /// </summary>
        /// <remarks>
        /// true is returned as <see cref="Visibility.Visible"/> but false is returned as <see cref="Visibility.Collapsed"/>.
        /// <see cref="Visibility.Hidden"/> is not supported.
        /// </remarks>
        /// <param name="value">The bool value to convert</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">If provided as "inverse" inverts the bool value</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object? Convert(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            var state = (bool)value;

            bool inverse = parameter?.ToString() == "inverse";

            return inverse ? (!state).ToVisibility() : state.ToVisibility();
        }

        /// <summary>
        /// Converts from <see cref="Visibility"/> to bool
        /// </summary>
        /// <param name="value">The Visibility value to convert</param>
        /// <param name="targetType">Ignored</param>
        /// <param name="parameter">If provided as "inverse" inverts the bool value</param>
        /// <param name="culture">Ignored</param>
        /// <returns></returns>
        public object? ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
        {
            var state = (Visibility)value;

            bool inverse = parameter?.ToString() == "inverse";

            var vals = state == Visibility.Visible;

            return inverse ? !vals : vals;
        }
    }
}