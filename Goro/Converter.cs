using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Thismaker.Goro
{
    [ValueConversion(typeof(IconType), typeof(string))]
    public class IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (parameter != null)
            {
                if ((string)parameter == "Status")
                {
                    var status = (StatusInfo)value;
                    return status switch
                    {
                        StatusInfo.Error => IconType.StatusCircleErrorX,
                        StatusInfo.Information => IconType.StatusCircleInfo,
                        StatusInfo.Success => IconType.StatusCircleCheckmark,
                        StatusInfo.Warning => IconType.StatusCircleExclamation,
                        _=>IconType.StatusCircleInfo
                    };
                }
            }
           

            var icon = (IconType)value;

            return icon switch
            {
                IconType.AreaChart=> "\uE9D2",
                IconType.Bluetooth => "\uE76D",
                IconType.Calendar=> "\uE787",
                IconType.ChatBubbles=> "\uE8F2",
                IconType.Connect => "\uE76D",
                IconType.ContactInfo=> "\uE779",
                IconType.ContactInfoMirrored=> "\uEA4A",
                IconType.FolderHorizontal => "\uF12B",
                IconType.GlobalNavigationButton => "\uE700",
                IconType.Setting=> "\uE713",
                IconType.SignOut=> "\uF3B1",
                IconType.StatusCircleBlock=> "\uF140",
                IconType.StatusCircleBlock2 => "\uF141",
                IconType.StatusCircleCheckmark => "\uF13E",
                IconType.StatusCircleErrorX => "\uF13D",
                IconType.StatusCircleExclamation => "\uF13C",
                IconType.StatusCircleInfo => "\uF13F",
                IconType.StatusCircleInner => "\uF137",
                IconType.StatusCircleQuestionMark => "\uF142",
                IconType.StatusCircleRing => "\uF138",
                IconType.StatusTriangleExclamation => "\uF13B",
                IconType.StatusTriangleInner => "\uF13A",
                IconType.StatusTriangleOuter => "\uF139",
                IconType.Wifi => "\uE76D",
                _ => "\uE76D",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public enum IconType
    {
        AreaChart,
        Bluetooth,
        Calendar,
        ChatBubbles,
        Connect,
        ContactInfo,
        ContactInfoMirrored,
        FolderHorizontal,
        GlobalNavigationButton,
        Setting,
        SignOut,
        StatusCircleBlock,
        StatusCircleBlock2,
        StatusCircleCheckmark,
        StatusCircleErrorX,
        StatusCircleExclamation,
        StatusCircleInfo,
        StatusCircleInner,
        StatusCircleQuestionMark,
        StatusCircleRing,
        StatusTriangleExclamation,
        StatusTriangleInner,
        StatusTriangleOuter,
        Wifi,
        
    }
}
