using System;
using System.Collections.Generic;
using System.Text;

namespace Thismaker.Core
{
    public static class DeltaTime
    {
        static DeltaTime()
        {
            Capture();
        }
        private static DateTime _now;

        public static void Capture()
        {
            _now = DateTime.Now;
        }

        public static double Elapsed
        {
            get=> (DateTime.Now - _now).TotalMilliseconds;
        }
    }
}
