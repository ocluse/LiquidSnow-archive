using System;
using System.Globalization;
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
                        _ => IconType.StatusCircleInfo
                    };
                }
            }

            var icon = (IconType)value;

            return icon switch
            {
                IconType.Add => "\uE710",
                IconType.AreaChart => "\uE9D2",
                IconType.Bluetooth => "\uE76D",
                IconType.Calendar => "\uE787",
                IconType.ChatBubbles => "\uE8F2",
                IconType.ChromeClose => "\uE8BB",
                IconType.Connect => "\uE76D",
                IconType.ContactInfo => "\uE779",
                IconType.ContactInfoMirrored => "\uEA4A",
                IconType.Delete => "\uE74D",
                IconType.Edit => "\uE70F",
                IconType.FolderHorizontal => "\uF12B",
                IconType.GlobalNavigationButton => "\uE700",
                IconType.OpenFile => "\uE8E5",
                IconType.Rename => "\uE8AC",
                IconType.Setting => "\uE713",
                IconType.Share => "\uE72D",
                IconType.SignOut => "\uF3B1",
                IconType.StatusCircleBlock => "\uF140",
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
                IconType.Print => "\uE749",
                IconType.Scan => "\uE8FE",
                IconType.Comment => "\uE90A",
                IconType.MultiSelect => "\uE762",
                IconType.Send => "\uE724",
                IconType.Italic => "\uE8DB",
                IconType.Bold => "\uE8DD",
                IconType.Underline => "\uE8DC",
                IconType.AlignCenter => "\uE8E3",
                IconType.AlignLeft => "\uE8E4",
                IconType.AlignRight => "\uE8E2",
                IconType.FontDecrease => "\uE8E7",
                IconType.FontIncrease => "\uE8E8",
                IconType.FontSize => "\uE8E9",
                IconType.FontColor => "\uE8D3",
                IconType.Font => "\uE8D2",
                IconType.BulletedList => "\uE8FD",
                IconType.Strikethrough => "\uEDE0",
                IconType.Undo => "\uE7A7",
                IconType.Redo => "\uE7A6",
                IconType.Copy => "\uE8C8",
                IconType.Paste => "\uE77F",
                IconType.Cut => "\uE8C6",

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
        Add,
        AreaChart,
        Bluetooth,
        Calendar,
        ChatBubbles,
        ChromeClose,
        Connect,
        ContactInfo,
        ContactInfoMirrored,
        Delete,
        Edit,
        FolderHorizontal,
        GlobalNavigationButton,
        OpenFile,
        Rename,
        Setting,
        Share,
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
        Print,
        Scan,
        Comment,
        MultiSelect,
        Send,
        Bold,
        Italic,
        Underline,
        AlignRight,
        AlignCenter,
        AlignLeft,
        FontDecrease,
        FontIncrease,
        FontSize,
        Font,
        FontColor,
        BulletedList,
        Strikethrough,
        Undo,
        Redo,
        Copy,
        Cut,
        Paste,
    }
}
