using System;
using System.Globalization;
using System.Windows.Data;

namespace Thismaker.Goro.Converters
{
    /// <summary>
    /// A converter that converts <see cref="DateTime"/> to a neatly formatted string.
    /// </summary>
    /// <remarks>
    /// This converter accepts a parameter that has to be specifically "D" that discards the time component
    /// of the DateTime, reporting only the date. Modify the static properties to customize for various languages.
    /// </remarks>
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

        /// <summary>
        /// The culture to use when performing the conversion.
        /// </summary>
        public static CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// If provided, the converter will always convert all DateTime values to local time first.
        /// </summary>
        public static bool ConvertToLocalTime { get; set; } = true;

        /// <summary>
        /// Converts a DateTime value to string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">If "D" discards the time component</param>
        /// <param name="culture">Ignored, use <see cref="Culture"/> instead to set the culture</param>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            var date = ((DateTime)value);

            if (ConvertToLocalTime)
            {
                date=date.ToLocalTime();
            }

            var onlyDate = parameter?.ToString() == "D";

            string result;

            if (date.Date == DateTime.Today)
            {
                result = TodayText;
            }
            else if (date.Date == DateTime.Today.AddDays(1))
            {
                result = TomorrowText;
            }
            else if (date.Date == DateTime.Today.AddDays(-1))
            {
                result = YesterdayText;
            }
            else if (DateTime.Today.AddDays(7) > date.Date || DateTime.Today.AddDays(-7) < date.Date)
            {
                if (date < DateTime.Today)
                {
                    result = LastPrefixed ?
                        $"{LastText}{LastSeparator}{date:dddd}" :
                        $"{date:dddd}{LastSeparator}";
                }
                else
                {
                    result = NextPrefixed ? $"{NextText}{NextSeparator}{date:dddd}" : $"{date:dddd}{NextSeparator}";
                }
            }
            else
            {
                result = date.ToString("dddd, MMM d, yyyy", Culture.DateTimeFormat);
            }

            return onlyDate ? result : $"{result}, {date:hh:mm tt}";
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}