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
            if (value == null)
            {
                return null;
            }

            DateTime date = ((DateTime)value).ToLocalTime();

            string datePart;

            if (date.Date == DateTime.Today)
            {
                datePart = "Today";
            }
            else if (date.Date == DateTime.Today.AddDays(1))
            {
                datePart = "Tomorrow";
            }
            else if (date.Date == DateTime.Today.AddDays(-1))
            {
                datePart = "Yesterday";
            }
            else if (Math.Abs((DateTime.Today - date.Date).TotalDays) < 7)
            {
                string prefix = date.Date < DateTime.Today ? "Last" : "Next";
                datePart = $"{prefix} {date:dddd}";
            }
            else
            {
                datePart = $"{date: dddd, MMM M, yyyy}";
            }

            return (parameter == null || parameter.ToString() != "d") ?
                $"{datePart}, {date:hh:mm tt}" :
                datePart;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}