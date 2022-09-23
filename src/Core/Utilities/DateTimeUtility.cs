using System;
using System.Globalization;

namespace Thismaker.Core.Utilities
{
    /// <summary>
    /// Contains utility methods for date time.
    /// </summary>
    public static class DateTimeUtility
    {
        /// <summary>
        /// Returns the first date of a week in a year.
        /// </summary>
        /// <param name="year">The year whose date we need</param>
        /// <param name="weekOfYear">The week itself, such as week 13</param>
        /// <returns></returns>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }

        /// <summary>
        /// Gets the week range of a DateTime, i.e the first day of that date's week and the last.
        /// </summary>
        /// <param name="startDate">The date whose week is to be obtained.</param>
        /// <param name="firstDay">The day to be used as first day of the week</param>
        /// <param name="weekRange">The number of days  in the week.</param>
        /// <returns>A tuple containing the first and last day the week where date time object is found.</returns>
        public static Tuple<DateTime, DateTime> GetWeek(DateTime startDate, DayOfWeek firstDay = DayOfWeek.Monday, int weekRange = 7)
        {
            while (startDate.DayOfWeek != firstDay)
            {
                startDate = startDate.AddDays(-1);
            }

            DateTime endDate = startDate.AddDays(weekRange);

            return Tuple.Create(startDate, endDate);
        }
    }
}
