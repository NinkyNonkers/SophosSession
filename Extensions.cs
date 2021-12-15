using System;

namespace SophosSessionHolder {
    public static class Extensions {
        public static int GetEpoch(this TimeSpan span) {
            TimeSpan t = span - new DateTime(1970, 1, 1).TimeOfDay;
            int secondsSinceEpoch = (int)t.TotalSeconds;
            return secondsSinceEpoch;
        }
    }
}