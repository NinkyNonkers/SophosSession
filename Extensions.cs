using System;

namespace SophosSessionHolder {
    public static class Extensions {
        public static int GetEpoch(this DateTime span) {
            double t = (span - new DateTime(1970, 1, 1)).TotalSeconds;
            return (int) t;
        }
    }
}