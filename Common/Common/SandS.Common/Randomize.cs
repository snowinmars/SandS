using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SandS.Common
{
    // ReSharper disable once AllowPublicClass
    public static class Randomize
    {
        private static readonly Random Random = new Random();

        public static bool Bool()
        {
            return Int(0, 2) == 0;
        }

        public static char Char(bool isFirstLetterUp = false)
        {
            return (char)Random.Next(97, 122);
        }

        public static double Double()
        {
            return Random.NextDouble();
        }

        public static double Double(double min, double max)
        {
            return (Random.NextDouble() * (max - min)) + min;
        }

        public static int Int()
        {
            return Random.Next();
        }

        public static int Int(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static string Json()
        {
            var sb = new StringBuilder();

            var propertiesCount = Int(4, 10);

            sb.Append("{");

            for (var i = 0; i < propertiesCount; i++)
            {
                sb.Append($"\"{String(16)}\":\"{String(16)}\"");

                if (i < propertiesCount)
                {
                    sb.Append(",");
                }
            }

            sb.Append("}");

            return sb.ToString();
        }

        public static double NegativeDouble(double threshold)
        {
            return -PositiveDouble(threshold);
        }

        public static int NegativeInt(int threshold)
        {
            return -PositiveInt(threshold);
        }

        public static double PositiveDouble(double threshold)
        {
            return Double(0, threshold);
        }

        public static int PositiveInt(int threshold)
        {
            return Int(0, threshold);
        }

        public static bool Roll(int percentChance)
        {
            switch (percentChance)
            {
            case 0:

                return false;

            case 100:

                return true;

            case int n when n > 0 && n < 100:

                return Int(0, 100) <= percentChance;

            default:

                throw new ArgumentOutOfRangeException(nameof(percentChance),
                                                      percentChance,
                                                      "Percent chance can't be more than 100");
            }
        }

        public static IEnumerable<T> Enumerable<T>(Func<T> getElement, int length)
        {
            for (var i = 0; i < length; i++)
            {
                yield return getElement();
            }
        }

        public static T[] Array<T>(Func<T> getElement, int length)
        {
            return Enumerable(getElement, length).ToArray();
        }

        public static string String(bool isFirstLetterUp = false)
        {
            return String(Int(16, 32), isFirstLetterUp);
        }

        public static string String(int length, bool isFirstLetterUp = false)
        {
            var sb = new StringBuilder(length);

            for (var j = 0; j < length; j++)
            {
                sb.Append(Char());
            }

            if (isFirstLetterUp)
            {
                sb[0] = char.ToUpper(sb[0], CultureInfo.CurrentCulture);
            }

            return sb.ToString();
        }
    }
}