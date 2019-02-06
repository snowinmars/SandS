using System;

namespace IComparableExtension.Extensions
{
    public static class Extensions
    {
        public static bool IsBetween<T>(this T number, T lhs, T rhs)
            where T : IComparable<T>
        {
            if (lhs.CompareTo(rhs) < 0)
            {
                return number.CompareTo(lhs) >= 0 &&
                       number.CompareTo(rhs) <= 0;
            }

            return number.CompareTo(rhs) >= 0 &&
                   number.CompareTo(lhs) <= 0;
        }

        /// <summary>
        /// if this if more that cutoff, return cutoff, otherwise return this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static T LimitFromAbove<T>(this T value, T limit)
            where T : IComparable
        {
            if (value.CompareTo(limit) > 0)
            {
                value = limit;
            }

            return value;
        }

        /// <summary>
        /// if this if less that cutoff, return cutoff, otherwise return this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static T LimitFromBelow<T>(this T value, T limit)
            where T : IComparable
        {
            if (value.CompareTo(limit) < 0)
            {
                value = limit;
            }

            return value;
        }
    }
}
