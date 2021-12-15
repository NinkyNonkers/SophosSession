namespace SophosSessionHolder {
    public static class Extensions {
        public static int GetEpoch(this TimeSpan span) {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            return secondsSinceEpoch;
        }
    }
}