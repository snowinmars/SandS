using System;

namespace DateTimeExtension.Extensions
{
    public static partial class Extensions
    {
        private static readonly DateTime UnixZeroTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromUnixTime(this long unixTime)
        {
            return UnixZeroTime.AddSeconds(unixTime);
        }

        public static long ToUnixTime(this DateTimeOffset date)
        {
            return Convert.ToInt64((date.UtcDateTime - UnixZeroTime).TotalSeconds);
        }
    }
}
