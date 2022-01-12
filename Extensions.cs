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
                if (msg.Content == null) 
                    throw new Exception("Did not receive proper response");
                if (msg.Content.ToString().ToLower().Contains("invalid user name")) 
                    throw new Exception("Invalid username or password!");
            }
            catch (Exception e) {
                ConsoleLogger.LogError(e.Message);
                return false;
            }
            return true;
        }
    }
}