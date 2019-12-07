using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SandS.Common
{
    // ReSharper disable once AllowPublicClass
    public static class Randomize
    {
        private const TextSettings DefaultTextSettings = TextSettings.AllowSmallLetters;

        private static readonly Random Random = new Random();

        public static bool Bool()
        {
            return Int(0, 2) == 0;
        }

        public static char Char(TextSettings settings = DefaultTextSettings)
        {
            var smallLettersRange = (min: 97, max: 122);
            var bigLettersRange = (min: 65, max: 90);
            var numbersRange = (min: 48, max: 57);
            var spaceRange = (min: 32, max: 32);

            var punctuationRanges = new[]
            {
                (min: 33, max: 47), (min: 58, max: 64), (min: 91, max: 96), (min: 123, max: 126),
            };

            var ranges = new List<(int min, int max)>();

            if (settings.HasFlag(TextSettings.AllowSmallLetters))
            {
                ranges.Add(smallLettersRange);
            }

            if (settings.HasFlag(TextSettings.AllowBigLetters))
            {
                ranges.Add(bigLettersRange);
            }

            if (settings.HasFlag(TextSettings.AllowPunctuation))
            {
                ranges.AddRange(punctuationRanges);
            }

            if (settings.HasFlag(TextSettings.AllowNumbers))
            {
                ranges.Add(numbersRange);
            }

            if (settings.HasFlag(TextSettings.AllowSpace))
            {
                ranges.Add(spaceRange);
            }

            var collapsedRange = new List<int>();

            foreach (var (min, max) in ranges)
            {
                for (var i = min; i < max; i++)
                {
                    collapsedRange.Add(i);
                }
            }

            var c = (char)From(collapsedRange);

            if (settings.HasFlag(TextSettings.IsFirstLetterUp))
            {
                c = char.ToUpperInvariant(c);
            }

            return c;
        }

        public static T From<T>(ICollection<T> collection)
        {
            return collection.ElementAt(Int(0, collection.Count - 1));
        }

        public static double Double()
        {
            return Random.NextDouble();
        }

        public static double Double(double min, double max)
        {
            if (min > max)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            return (Random.NextDouble() * (max - min)) + min;
        }

        public static int Int()
        {
            return Random.Next();
        }

        public static int Int(int min, int max)
        {
            if (min > max)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

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
            if (threshold < 0)
            {
                threshold = -threshold;
            }

            return Double(0, threshold);
        }

        public static int PositiveInt(int threshold)
        {
            if (threshold < 0)
            {
                threshold = -threshold;
            }

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

        public static string String(TextSettings settings = DefaultTextSettings)
        {
            return String(Int(16, 32), settings);
        }

        public static string String(int length, TextSettings settings = DefaultTextSettings)
        {
            return String(length, length, settings);
        }

        public static string String(int minLength, int maxLength, TextSettings settings = DefaultTextSettings)
        {
            var length = Int(minLength, maxLength);
            var sb = new StringBuilder(length);

            var isFirstLetterUp = settings.HasFlag(TextSettings.IsFirstLetterUp);
            settings = settings & ~TextSettings.IsFirstLetterUp;

            for (var j = 0; j < length; j++)
            {
                sb.Append(Char(settings));
            }

            if (isFirstLetterUp)
            {
                sb[0] = char.ToUpperInvariant(sb[0]);
            }

            return sb.ToString();
        }

        [Flags]
        public enum TextSettings
        {
            IsFirstLetterUp = 1,

            AllowSmallLetters = 2,

            AllowBigLetters = 4,

            AllowPunctuation = 8,

            AllowNumbers = 16,

            AllowSpace = 32,

            AllowNonAsciiChars, // TODO [snowinmars]
        }
    }
}