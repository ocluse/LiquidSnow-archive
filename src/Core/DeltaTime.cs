using System;

namespace Thismaker.Core
{
    /// <summary>
    /// Contains utility methods for determining the interval of time between two events.
    /// </summary>
    public static class DeltaTime
    {
        static DeltaTime()
        {
            Capture();
        }
        private static DateTime _now;

        /// <summary>
        /// Sets the delta time object to start counting the intervals
        /// </summary>
        public static void Capture()
        {
            _now = DateTime.Now;
        }

        /// <summary>
        /// Returns the time that has elapsed in milliseconds since the last <see cref="Capture"/> was called
        /// </summary>
        public static double Elapsed
        {
            get=> (DateTime.Now - _now).TotalMilliseconds;
        }
    }
}
