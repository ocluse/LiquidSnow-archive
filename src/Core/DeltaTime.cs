using System;

namespace Thismaker.Core
{
    /// <summary>
    /// A simple static class that can be used to tell the interval between two periods.
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
        /// Returns the time that has elapsed in miliseconds since the last <see cref="Capture"/> was called
        /// </summary>
        public static double Elapsed
        {
            get=> (DateTime.Now - _now).TotalMilliseconds;
        }
    }
}
