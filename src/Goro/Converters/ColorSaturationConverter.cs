using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Thismaker.Goro.Utilities;
using Color = System.Windows.Media.Color;

namespace Thismaker.Goro
{
    /// <summary>
    /// A converter that modifies the saturation of a provided <see cref="Color"/> or <see cref="SolidColorBrush"/>.
    /// Adding the parameter "L" lightens the color, while "W" desaturates the color, and "D" darkens the color.
    /// Without a paramter, the converter defaults to lightening "L"
    /// </summary>
    [ValueConversion(typeof(Color), typeof(Color), ParameterType=typeof(string))]
    [ValueConversion(typeof(SolidColorBrush), typeof(SolidColorBrush), ParameterType =typeof(string))]
    [ValueConversion(typeof(SolidColorBrush), typeof(Color), ParameterType = typeof(string))]
    [ValueConversion(typeof(Color), typeof(SolidColorBrush), ParameterType = typeof(string))]
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

            if (targetType == typeof(Color)) return color;

            return ColorUtility.CreateBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}