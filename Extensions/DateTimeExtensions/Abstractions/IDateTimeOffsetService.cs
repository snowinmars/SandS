using System;

using DateTimeExtension.Entities;

namespace DateTimeExtension.Abstractions
{
    public interface IDateTimeOffsetService
    {
        DateTimeOffset GetCurrentDateUtc();

        DateTimeOffset GetCurrentDateTimeUtc();

        TimeZoneInfo[] ListTimeZones();

        TimeZoneInfo GetTimeZone(string id);

        TimeZoneInfo GetDefaultTimeZone();

        /// <summary>
        /// Changes offset for DateTimeOffset to locationTimeZone offset, if the location exists.
        /// If the location doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).
        /// </summary>
        /// <param name="offset">Offset in any timeZone.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        /// <returns>DateTimeOffset in the destination location timeZone.</returns>
        DateTimeOffset ChangeOffset(DateTimeOffset offset, string destinationLocationTimeZoneId);

        /// <summary>
        /// Changes offset for DateTimeOffset to locationTimeZone offset, if the location exists.
        /// If the location doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).
        /// </summary>
        /// <param name="offset">Offset in any timeZone.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        /// <returns>DateTimeOffset in the destination location timeZone or null.</returns>
        DateTimeOffset? ChangeOffset(DateTimeOffset? offset, string destinationLocationTimeZoneId);

        /// <summary>
        /// Convert location DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="locationTime">Can't have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="locationTimeZoneId">Location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the location timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        DateTimeOffset ConvertLocationTimeToOffset(DateTime locationTime, string locationTimeZoneId);

        /// <summary>
        /// Convert location DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="locationTime">Can't have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="locationTimeZoneId">Location timezone id. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the location timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        DateTimeOffset? ConvertLocationTimeToOffset(DateTime? locationTime, string locationTimeZoneId);

        /// <summary>
        /// Convert UTC DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="utcTime">Must have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If this parameter is null or empty, method will use the Utc timeZone. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the destination timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind not equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        DateTimeOffset ConvertUtcTimeToOffset(DateTime utcTime, string destinationLocationTimeZoneId = null);

        /// <summary>
        /// Convert UTC DateTime to DateTimeOffset with given location timeZone.
        /// </summary>
        /// <param name="utcTime">Must have Kind equals to DateTimeKind.Utc.</param>
        /// <param name="destinationLocationTimeZoneId">Destination location timezone id. If this parameter is null or empty, method will use the Utc timeZone. If the timezone doesn't exists, uses DefaultTimeZone (AUS Eastern Standard Time).</param>
        /// <returns>DateTimeOffset in the destination timeZone.</returns>
        /// <exception cref="InvalidOperationException">If locationTime had Kind not equals to DateTimeKind.Utc.</exception>
        /// <exception cref="DaylightSavingTimeException">Throws if the source offset and the destination offset have different values of the Daylight Saving Time Switch.</exception>
        DateTimeOffset? ConvertUtcTimeToOffset(DateTime? utcTime, string destinationLocationTimeZoneId = null);

        DateTimeOffset GetCurrentDateTimeForTimeZone(string locationTimeZoneId);

        bool IsLeftGreaterRight(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset);

        bool IsLeftGreaterRight(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo);

        bool IsLeftGreaterRight(DateTimeOffset lhs, DateTimeOffset rhs);

        bool IsLeftGreaterRight(DateTime lhs, DateTime rhs);

        bool AreEquals(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset);

        bool AreEquals(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo);

        bool AreEquals(DateTimeOffset lhs, DateTimeOffset rhs);

        bool AreEquals(DateTime lhs, DateTime rhs);

        bool IsLeftLessRight(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset);

        bool IsLeftLessRight(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo);

        bool IsLeftLessRight(DateTimeOffset lhs, DateTimeOffset rhs);

        bool IsLeftLessRight(DateTime lhs, DateTime rhs);

        int Compare(DateTimeOffset offset, DateTime dateTime, TimeZoneInfo timeZoneInfo);

        int Compare(DateTime dateTime, TimeZoneInfo timeZoneInfo, DateTimeOffset offset);

        int Compare(DateTimeOffset lhs, DateTimeOffset rhs);

        int CompareDateTime(DateTime dateTimeUtc, DateTime offsetUtc);
    }
}
