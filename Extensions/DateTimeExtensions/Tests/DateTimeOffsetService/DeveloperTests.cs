using System;

using DateTimeExtension.Abstractions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using DateTimeExtension.Entities;
using DateTimeExtension.Services;

namespace Tests
{
    // TODO: [DT] these tests are intended to be developer's helpers. They are not real 'tests', they are data generators. If we don't need the dateTimeOffsetService, we should remove it and these tests
    [Ignore]
    [TestClass]
    public class DateTimeOffsetServiceTests
    {
        private static readonly TimeZoneInfo[] timeZoneInfos;

        private static readonly DateTime[] dateTimeUtcs;

        private IDateTimeOffsetService dateTimeOffsetService;

        static DateTimeOffsetServiceTests()
        {
            timeZoneInfos = new[]
            {
                TimeZoneInfo.FindSystemTimeZoneById("GTB Standard Time"), // UTC+2
                TimeZoneInfo.Utc,
                TimeZoneInfo.FindSystemTimeZoneById("Aleutian Standard Time"), // UTC-10
            };

            var now = DateTime.UtcNow;

            dateTimeUtcs = new[]
            {
                now.AddDays(-7),
                now,
                now.AddDays(12),
            };
        }

        [TestInitialize]
        public void TestInitialize()
        {
            dateTimeOffsetService = new DateTimeOffsetService();
        }

        [TestMethod]
        public void NewConvertFromUtcToUtcShouldDoNothing()
        {
            foreach (var timeZoneInfo in timeZoneInfos)
            {
                foreach (var dateTimeUtc in dateTimeUtcs)
                {
                    var offset = dateTimeOffsetService.ConvertUtcTimeToOffset(dateTimeUtc, timeZoneInfo.Id);

                    bool areEquals = dateTimeOffsetService.AreEquals(
                        dateTimeUtc,
                        offset.UtcDateTime);

                    Assert.IsTrue(areEquals);

                    var utcOffset = dateTimeOffsetService.ChangeOffset(offset, TimeZoneInfo.Utc.Id);
                    var locationOffset = dateTimeOffsetService.ChangeOffset(utcOffset, timeZoneInfo.Id);

                    areEquals = dateTimeOffsetService.AreEquals(
                        dateTimeUtc,
                        locationOffset.UtcDateTime);

                    Assert.IsTrue(areEquals);
                }
            }
        }


        [TestMethod]
        public void ConvertUtcToLocationTimeWithDaylightSavingShouldReturnEqualValues()
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Mid-Atlantic Standard Time");
            var dateTimeUtc = DateTime.Parse("9/27/2065 2:45:40 AM");
            dateTimeUtc = DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);

            bool isSuccessed = false;

            try
            {
                var unused = dateTimeOffsetService.ConvertUtcTimeToOffset(dateTimeUtc, timeZoneInfo.Id);
            }
            catch (DaylightSavingTimeException)
            {
                isSuccessed = true;
            }

            Assert.IsTrue(isSuccessed);
        }

        [TestMethod]
        public void ChangeOffsetTimeZoneShouldWorkAsConvertTimeBySystemTimeZoneId()
        {
            foreach (var timeZoneInfo1 in timeZoneInfos)
            {
                foreach (var timeZoneInfo2 in timeZoneInfos)
                {
                    foreach (var dateTimeUtc in dateTimeUtcs)
                    {
                        var locationOffset1 = dateTimeOffsetService.ConvertUtcTimeToOffset(dateTimeUtc, timeZoneInfo1.Id);
                        var locationOffset2 = dateTimeOffsetService.ChangeOffset(locationOffset1, timeZoneInfo2.Id);
                    }
                }
            }
        }

        [TestMethod]
        public void ChangeOffsetTimeZoneWithDaylightSavingShouldWorkAsConvertTimeBySystemTimeZoneId()
        {
            var timeZoneInfo1 = TimeZoneInfo.FindSystemTimeZoneById("Bougainville Standard Time");
            var timeZoneInfo2 = TimeZoneInfo.FindSystemTimeZoneById("Alaskan Standard Time");
            var dateTimeUtc = DateTime.Parse("11/5/2090 9:28:32 AM");
            dateTimeUtc = DateTime.SpecifyKind(dateTimeUtc, DateTimeKind.Utc);

            bool isSuccessed = false;

            try
            {
                var locationOffset1 = dateTimeOffsetService.ConvertUtcTimeToOffset(dateTimeUtc, timeZoneInfo1.Id);
                var locationOffset2 = dateTimeOffsetService.ChangeOffset(locationOffset1, timeZoneInfo2.Id);
            }
            catch (DaylightSavingTimeException)
            {
                isSuccessed = true;
            }

            Assert.IsTrue(isSuccessed);
        }

        [TestMethod]
        public void GreaterOperation()
        {
            var now = DateTime.UtcNow;
            var lessDateTime = now.AddDays(-5);
            var greaterDateTime = now.AddDays(7);

            var lessDateTimeOffset = new DateTimeOffset(lessDateTime, TimeZoneInfo.Utc.BaseUtcOffset);
            var greaterDateTimeOffset = new DateTimeOffset(greaterDateTime, TimeZoneInfo.Utc.BaseUtcOffset);

            bool result;

            result = dateTimeOffsetService.IsLeftGreaterRight(lessDateTime, TimeZoneInfo.Utc, greaterDateTimeOffset);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.IsLeftGreaterRight(greaterDateTimeOffset, lessDateTime, TimeZoneInfo.Utc);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.IsLeftGreaterRight(lessDateTimeOffset, greaterDateTimeOffset);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.IsLeftGreaterRight(greaterDateTimeOffset, lessDateTimeOffset);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.IsLeftGreaterRight(lessDateTime, greaterDateTime);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.IsLeftGreaterRight(greaterDateTime, lessDateTime);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EqualsOperation()
        {
            var now = DateTime.UtcNow;
            var equalsDateTime1 = now.AddDays(7);
            var equalsDateTime2 = now.AddDays(7);
            var nonEqualsDateTime = now.AddDays(8);

            var equalsDateTimeOffset1 = new DateTimeOffset(equalsDateTime1, TimeZoneInfo.Utc.BaseUtcOffset);
            var equalsDateTimeOffset2 = new DateTimeOffset(equalsDateTime2, TimeZoneInfo.Utc.BaseUtcOffset);
            var nonEqualsDateTimeOffset = new DateTimeOffset(nonEqualsDateTime, TimeZoneInfo.Utc.BaseUtcOffset);

            bool result;

            result = dateTimeOffsetService.AreEquals(equalsDateTime1, equalsDateTime2);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.AreEquals(equalsDateTime2, equalsDateTime1);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.AreEquals(equalsDateTime1, nonEqualsDateTime);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.AreEquals(equalsDateTime2, nonEqualsDateTime);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.AreEquals(equalsDateTimeOffset1, equalsDateTimeOffset2);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.AreEquals(equalsDateTimeOffset2, equalsDateTimeOffset1);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.AreEquals(equalsDateTimeOffset1, nonEqualsDateTimeOffset);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.AreEquals(equalsDateTimeOffset2, nonEqualsDateTimeOffset);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void LessOperation()
        {
            var now = DateTime.UtcNow;
            var lessDateTime = now.AddDays(-7);
            var greaterDateTime = now.AddDays(-5);

            var lessDateTimeOffset = new DateTimeOffset(lessDateTime, TimeZoneInfo.Utc.BaseUtcOffset);
            var greaterDateTimeOffset = new DateTimeOffset(greaterDateTime, TimeZoneInfo.Utc.BaseUtcOffset);

            bool result;

            result = dateTimeOffsetService.IsLeftLessRight(lessDateTime, TimeZoneInfo.Utc, greaterDateTimeOffset);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.IsLeftLessRight(greaterDateTimeOffset, lessDateTime, TimeZoneInfo.Utc);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.IsLeftLessRight(lessDateTimeOffset, greaterDateTimeOffset);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.IsLeftLessRight(greaterDateTimeOffset, lessDateTimeOffset);
            Assert.IsFalse(result);

            result = dateTimeOffsetService.IsLeftLessRight(lessDateTime, greaterDateTime);
            Assert.IsTrue(result);

            result = dateTimeOffsetService.IsLeftLessRight(greaterDateTime, lessDateTime);
            Assert.IsFalse(result);
        }

        private void EnsureDaylightSavingIsStillSame(DateTime utcTime, DateTime locationTime, TimeZoneInfo timeZoneInfo)
        {
            if (utcTime.Kind != DateTimeKind.Utc)
            {
                throw new InvalidOperationException("Utc time should have Utc kind");
            }

            if ((timeZoneInfo.IsDaylightSavingTime(utcTime) && !timeZoneInfo.IsDaylightSavingTime(locationTime))
                || (!timeZoneInfo.IsDaylightSavingTime(utcTime) && timeZoneInfo.IsDaylightSavingTime(locationTime)))
            {
                throw new DaylightSavingTimeException("This exception means that the daylight saving toggles. You should handle this situation with a catch directive");
            }
        }
    }
}
