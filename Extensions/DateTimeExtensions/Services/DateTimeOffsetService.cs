using System;
using System.Linq;

using DateTimeExtension.Abstractions;
using DateTimeExtension.Entities;

namespace DateTimeExtension.Services
{
    public class DateTimeOffsetService : IDateTimeOffsetService
    {
        private const string DefaultTimeZoneId = "AUS Eastern Standard Time";

        public DateTimeOffset GetCurrentDateUtc()
        {
            return DateTimeOffset.UtcNow.UtcDateTime.Date;
        }

        public DateTimeOffset GetCurrentDateTimeUtc()
        {
            return DateTimeOffset.UtcNow.UtcDateTime;
        }

        public TimeZoneInfo[] ListTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().ToArray();
        }

        public TimeZoneInfo GetTimeZone(string id)
        {
            return ListTimeZones().FirstOrDefault(x => string.CompareOrdinal(x.Id, id) == 0);
        }

        public TimeZoneInfo GetDefaultTimeZone()
        {
            return GetTimeZone(DefaultTimeZoneId);
        }

        /// <summary>
        /// Changes offset for DateTimeOffset to locationTimeZone offset, if the location exists.
        /// If the location doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).
        /// </summary>
        /// <param name="offset">Offset in any timeZone.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        /// <returns>DateTimeOffset in the destination location timeZone or null.</returns>
        public DateTimeOffset? ChangeOffset(DateTimeOffset? offset, string destinationLocationTimeZoneId)
        {
            if (!offset.HasValue)
            {
                return null;
            }

            return ChangeOffset(offset.Value, destinationLocationTimeZoneId);
        }

        /// <summary>
        /// Changes offset for DateTimeOffset to locationTimeZone offset, if the location exists.
        /// If the location doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).
        /// </summary>
        /// <param name="offset">Offset in any timeZone.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        /// <returns>DateTimeOffset in the destination location timeZone.</returns>
        public DateTimeOffset ChangeOffset(DateTimeOffset offset, string destinationLocationTimeZoneId)
        {
            var timeZoneInfo = GetTimeZone(destinationLocationTimeZoneId) ?? GetTimeZone(DefaultTimeZoneId);

            var utcOffset = new DateTimeOffset(offset.UtcDateTime, TimeZoneInfo.Utc.BaseUtcOffset);

            var locationOffset = TimeZoneInfo.ConvertTime(utcOffset, timeZoneInfo);

            EnsureDaylightSavingIsStillSame(utcOffset, locationOffset, timeZoneInfo);

            return locationOffset;
        }

        /// <summary>
        /// Convert location DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="locationTime">Can't have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="locationTimeZoneId">Location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the location timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        public DateTimeOffset? ConvertLocationTimeToOffset(DateTime? locationTime, string locationTimeZoneId)
        {
            if (!locationTime.HasValue)
            {
                return null;
            }

            return ConvertLocationTimeToOffset(locationTime.Value, locationTimeZoneId);
        }

        /// <summary>
        /// Convert location DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="locationTime">Can't have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="locationTimeZoneId">Location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the location timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        public DateTimeOffset ConvertLocationTimeToOffset(DateTime locationTime, string locationTimeZoneId)
        {
            if (locationTime.Kind == DateTimeKind.Utc)
            {
                throw new InvalidOperationException("Time supposed to be in local timezone");
            }

            var timeZoneInfo = GetTimeZone(locationTimeZoneId) ?? GetTimeZone(DefaultTimeZoneId);

            locationTime = EnsureDateIsValid(locationTime, timeZoneInfo); // TODO: [DT] do we need this?

            var utcTime = TimeZoneInfo.ConvertTimeToUtc(locationTime, timeZoneInfo);

            var utcOffset = new DateTimeOffset(utcTime, TimeZoneInfo.Utc.BaseUtcOffset);

            var locationOffset = TimeZoneInfo.ConvertTime(utcOffset, timeZoneInfo);

            EnsureDaylightSavingIsStillSame(utcOffset, locationOffset, timeZoneInfo);

            return locationOffset;
        }

        /// <summary>
        /// Convert UTC DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="utcTime">Must have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If this parameter is null or empty, method will use the Utc timeZone. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the destination timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind not equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        public DateTimeOffset? ConvertUtcTimeToOffset(DateTime? utcTime, string destinationLocationTimeZoneId = null)
        {
            if (!utcTime.HasValue)
            {
                return null;
            }

            return ConvertUtcTimeToOffset(utcTime.Value, destinationLocationTimeZoneId);
        }

        /// <summary>
        /// Convert UTC DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="utcTime">Must have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If this parameter is null or empty, method will use the Utc timeZone. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the destination timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind not equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        public DateTimeOffset ConvertUtcTimeToOffset(DateTime utcTime, string destinationLocationTimeZoneId = null)
        {
            if (utcTime.Kind != DateTimeKind.Utc)
            {
                throw new InvalidOperationException("Time supposed to be in UTC");
            }

            if (string.IsNullOrWhiteSpace(destinationLocationTimeZoneId))
            {
                destinationLocationTimeZoneId = TimeZoneInfo.Utc.Id;
            }

            var timeZoneInfo = GetTimeZone(destinationLocationTimeZoneId) ?? GetTimeZone(DefaultTimeZoneId);

            var utcOffset = new DateTimeOffset(utcTime, TimeZoneInfo.Utc.BaseUtcOffset);

            var locationOffset = TimeZoneInfo.ConvertTime(utcOffset, timeZoneInfo);

            EnsureDaylightSavingIsStillSame(utcOffset, locationOffset, timeZoneInfo);

            return locationOffset;
        }

        public DateTimeOffset GetCurrentDateTimeForTimeZone(string locationTimeZoneId)
        {
            return ConvertUtcTimeToOffset(GetCurrentDateTimeUtc().UtcDateTime, locationTimeZoneId);
        }

        #region comparer
        public bool IsLeftGreaterRight(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset)
        {
            return Compare(dateTime, timeZoneInfo, offset) > 0;
        }

        public bool IsLeftGreaterRight(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            return Compare(dateTime, timeZoneInfo, offset) <= 0;
        }

        public bool IsLeftGreaterRight(DateTimeOffset lhs, DateTimeOffset rhs)
        {
            return Compare(lhs, rhs) > 0;
        }

        public bool IsLeftGreaterRight(DateTime lhs, DateTime rhs)
        {
            return CompareDateTime(lhs, rhs) > 0;
        }

        public bool AreEquals(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset)
        {
            return Compare(dateTime, timeZoneInfo, offset) == 0;
        }

        public bool AreEquals(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            return AreEquals(dateTime, timeZoneInfo, offset);
        }

        public bool AreEquals(DateTimeOffset lhs, DateTimeOffset rhs)
        {
            return Compare(lhs, rhs) == 0;
        }

        public bool AreEquals(DateTime lhs, DateTime rhs)
        {
            return CompareDateTime(lhs, rhs) == 0;
        }

        public bool IsLeftLessRight(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset)
        {
            return Compare(dateTime, timeZoneInfo, offset) < 0;
        }

        public bool IsLeftLessRight(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            return Compare(dateTime, timeZoneInfo, offset) >= 0;
        }

        public bool IsLeftLessRight(DateTimeOffset lhs, DateTimeOffset rhs)
        {
            return Compare(lhs, rhs) < 0;
        }

        public bool IsLeftLessRight(DateTime lhs, DateTime rhs)
        {
            return CompareDateTime(lhs, rhs) < 0;
        }

        public int Compare(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo)
        {
            return Compare(dateTime, timeZoneInfo, offset);
        }

        public int Compare(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset)
        {
            if (timeZoneInfo.Id == TimeZoneInfo.Utc.Id)
            {
                return CompareDateTime(dateTime, offset.UtcDateTime);
            }

            var dateTimeUtc = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo);

            var offsetUtc = TimeZoneInfo.ConvertTimeToUtc(offset.DateTime, timeZoneInfo);

            return CompareDateTime(dateTimeUtc, offsetUtc);
        }

        public int Compare(DateTimeOffset lhs, DateTimeOffset rhs)
        {
            return CompareDateTime(lhs.UtcDateTime, rhs.UtcDateTime);
        }

        public int CompareDateTime(DateTime dateTimeUtc, DateTime offsetUtc)
        {
            return dateTimeUtc.CompareTo(offsetUtc);
        }
        #endregion comparer

        private DateTime EnsureDateIsValid(DateTime locationTime, TimeZoneInfo timeZoneInfo)
        {
            for (int i = 0; i < 24; i++)
            {
                if (!timeZoneInfo.IsInvalidTime(locationTime))
                {
                    break;
                }

                if (i == 23)
                {
                    return locationTime;
                }

                locationTime = locationTime.AddHours(1);
            }

            return locationTime;
        }

        internal void EnsureDaylightSavingIsStillSame(DateTimeOffset utcOffset, DateTimeOffset locationOffset, TimeZoneInfo timeZoneInfo)
        {
            if (timeZoneInfo.IsDaylightSavingTime(utcOffset.UtcDateTime) ^ timeZoneInfo.IsDaylightSavingTime(locationOffset.DateTime))
            {
                throw new DaylightSavingTimeException("This exception means that the daylight saving toggles. You should handle this situation with a catch directive");
            }
        }
    }
}
