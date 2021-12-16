using System;
using NinkyNonk.Shared.Logging;

namespace SophosSessionHolder {
    public static class Extensions {
        public static int GetEpoch(this DateTime span) {
            double t = (span - new DateTime(1970, 1, 1)).TotalSeconds;
            return (int) t;
        }

        public static bool CheckSuccess(this HttpResponseMessage msg) {
            try {
                msg.EnsureSuccessStatusCode();
            }
            catch (Exception e) {
                ConsoleLogger.LogError(e.Message);
                return false;
            }
            return true;
        }
    }
}