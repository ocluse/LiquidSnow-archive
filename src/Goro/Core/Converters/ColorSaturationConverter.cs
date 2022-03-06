using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Thismaker.Goro.Utilities;
using Color = System.Windows.Media.Color;

namespace Thismaker.Goro.Converters
{
    /// <summary>
    /// A converter that modifies the saturation of a provided <see cref="Color"/> or <see cref="SolidColorBrush"/>.
    /// </summary>
    /// <remarks>
    /// Adding the parameter "L" lightens the color, while "W" desaturates the color, and "D" darkens the color.
    /// Without a paramter, the converter defaults to lightening "L"
    /// </remarks>
    [ValueConversion(typeof(Color), typeof(Color), ParameterType=typeof(string))]
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush), ParameterType =typeof(string))]
    [ValueConversion(typeof(SolidColorBrush), typeof(Color), ParameterType = typeof(string))]
    [ValueConversion(typeof(Color), typeof(SolidColorBrush), ParameterType = typeof(string))]
    public class ColorSaturationConverter : IValueConverter
    {
        /// <summary>
        /// Saturates or desaturates a color, depending on the parameter passed
        /// </summary>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null;
            }

            var mode = "L";

            if (parameter is not null)
            {
                mode = parameter.ToString();

                if(mode is not "D" or "W")
                {
                    throw new ArgumentException("The parameter provided is not valid, please use L, D or W", nameof(parameter));
                }
            }

            Color? color = null;

            if(value is SolidColorBrush brush)
            {
                color = brush.Color;
            }
            else if(value is Color c)
            {
                color = c;
            }
            else if(value is string hexString)
            {
                if (hexString.StartsWith("#"))
                {
                    color = ColorUtility.Parse(hexString);
                }
            }

            if(color is null)
            {
                throw new ArgumentException("The provided value cannot be converted", nameof(value));
            }

            if (mode == "L")
            {
                color = ColorUtility.Lighten(color.Value);
            }
            else if (mode == "W")
            {
                color = ColorUtility.Desaturate(color.Value, 0.1f);
            }
            else
            {
                color = ColorUtility.Darken(color.Value);
            }

            if (targetType == typeof(Color)) return color;

            return ColorUtility.CreateBrush(color.Value);

        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}