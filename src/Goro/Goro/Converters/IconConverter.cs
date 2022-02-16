using System;
using System.Globalization;
using System.Windows.Data;

namespace Thismaker.Goro
{
    /// <summary>
    /// This converter is private to the library and is used by the <see cref="StatusIndicator"/>
    /// to show the appropriate icon.
    /// </summary>
    class IconConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (parameter == null) return null;

            if (parameter.GetType() == typeof(string))
            {
                if ((string)parameter == "Status")
                {
                    var status = (StatusInfo)value;
                    return status switch
                    {
                        StatusInfo.Error => Icon.Error,
                        StatusInfo.Information => Icon.Info,
                        StatusInfo.Success => Icon.Done,
                        StatusInfo.Warning => Icon.Warning,
                        _ => Icon.Info
                    };
                }

                //We don't know the request made here. Just return null
                return null;
            }

            //Get the type of parameter we are dealing with:
            if (parameter.GetType() == typeof(IconDesign))
            {
                var icon = (Icon)value;
                var scheme = (IconDesign)parameter;

                if (icon == Icon.None) return null;

                //SchemeBasedIcon:
                if (scheme == IconDesign.Segoe)
                {
                    return icon.ToSegoeCode();
                }
                else
                {
                    return IconManager.LoadIconAsBrush(icon, scheme);
                }
            }

            //Return null since we dont know what parameter we've been given:
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}