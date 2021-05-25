using System;
using System.Globalization;
using System.Windows.Data;

namespace Thismaker.Goro
{
    /// <summary>
    /// A converter that converts <see cref="DateTime"/> to a neatly formatted string.
    /// This converter accepts a parameter that has to be specifically "D" that discards the time component
    /// of the DateTime, reporting only the date. Modify the static properties to customize for various languages
    /// </summary>
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateTimeConverter : IValueConverter
    {
        /// <summary>
        /// The text to show when displaying the word Today.
        /// </summary>
        public static string TodayText { get; set; } = "Today";

        /// <summary>
        /// The text to show when displaying the word Yesterday
        /// </summary>
        public static string YesterdayText { get; set; } = "Yesterday";

        /// <summary>
        /// The text to show when dispalung the word Tomorrow
        /// </summary>
        public static string TomorrowText { get; set; } = "Tomorrow";

        /// <summary>
        /// The text representation of Last. The default is "Last"
        /// </summary>
        public static string LastText { get; set; } = "Last";

        /// <summary>
        /// The text representation of Next. The default is "Next"
        /// </summary>
        public static string NextText { get; set; } = "Next";

        /// <summary>
        /// A string for separating the last prefix/suffix from the day. The default is a single space
        /// </summary>
        public static string LastSeparator { get; set; } = " ";

        /// <summary>
        /// A string for separating the next prefix/suffix from the day. The default is a single space
        /// </summary>
        public static string NextSeparator { get; set; } = " ";

        /// <summary>
        /// If true, the <see cref="LastText"/> is shown before the day e.g Last Thursday.
        /// </summary>
        public static bool LastPrefixed { get; set; } = true;

        /// <summary>
        /// If true, the <see cref="NextText"/> is shown before the day e.g Next Wednesday.
        /// </summary>
        public static bool NextPrefixed { get; set; } = true;

        public static CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var date = ((DateTime)value).ToLocalTime();

            var dif = DateTime.Today - date;
            var days = dif.TotalDays;
            var onlyDate = parameter?.ToString() == "D";

            if (onlyDate)
            {
                if (date.Date == DateTime.Today)
                {
                    return TodayText;
                }
                if (days < 2)
                {
                    return date < DateTime.Today ? YesterdayText : TomorrowText;
                }
                if (days < 7)
                {
                    if (date < DateTime.Today)
                    {
                        return LastPrefixed ?
                            $"{LastText}{LastSeparator}{date:dddd}" :
                            $"{date:dddd}{LastSeparator}";
                    }
                    else
                    {
                        return NextPrefixed ?
                            $"{NextText}{NextSeparator}{date:dddd}" :
                            $"{date:dddd}{NextSeparator}";
                    }
                }

                return date.ToString("dddd, MMM M, yyyy", Culture.DateTimeFormat);
            }
            else
            {
                if (date.Date == DateTime.Today)
                {
                    return $"{TodayText}, {date:hh:mm tt}";
                }
                if (days < 2)
                {
                    var prefix = date < DateTime.Today ? YesterdayText : TomorrowText;
                    return $"{prefix} {date:hh:mm tt}";
                }
                if (days < 7)
                {
                    if (date < DateTime.Today)
                    {
                        return LastPrefixed ?
                            $"{LastText}{LastSeparator}{date:dddd, hh:mm tt}" :
                            $"{date:dddd}{LastSeparator}{date:, hh:mm tt}";
                    }
                    else
                    {
                        return NextPrefixed ?
                            $"{NextText}{NextSeparator}{date:dddd, hh:mm tt}" :
                            $"{date:dddd}{NextSeparator}{date:, hh:mm tt}";
                    }
                }

                return date.ToString("dddd, MMM M, yyyy, hh:mm tt", Culture.DateTimeFormat);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}