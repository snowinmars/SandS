using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringExtensions.Extensions
{
    public static class StringExtensions
    {
        public static bool DoesEndWithUpper(this string str)
        {
            return DoesNthCharacterUpper(str, str.Length - 1);
        }

        public static bool DoesStartWithUpper(this string str)
        {
            return DoesNthCharacterUpper(str, 0);
        }

        public static string FirstLetterToLower(this string str)
        {
            return ConvertNthLetterCase(str, 0, x => char.ToLower(x, CultureInfo.InvariantCulture));
        }

        public static string FirstLetterToUpper(this string str)
        {
            return ConvertNthLetterCase(str, 0, x => char.ToUpper(x, CultureInfo.InvariantCulture));
        }

        public static bool DoesConsistOfCyrillic(this string str)
        {
            return DoesConsistOfCyrillic(str, new char[0]);
        }

        public static bool DoesConsistOfCyrillic(this string str, char[] allowedChars)
        {
            if (allowedChars == null)
            {
                throw new ArgumentNullException(nameof(allowedChars));
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            var chars = ReplaceAllowedChars(str, allowedChars);

            // See https://msdn.microsoft.com/en-us/library/ms972966.aspx for info about \P key
            return
                Regex.IsMatch(chars, @"\P{IsCyrillic}") &&
                Regex.IsMatch(chars, @"\P{IsCyrillicSupplement}");
        }

        public static bool DoesConsistOfLatin(this string str)
        {
            return DoesConsistOfLatin(str, new char[0]);
        }

        public static bool DoesConsistOfLatin(this string str, char[] allowedChars)
        {
            if (allowedChars == null)
            {
                throw new ArgumentNullException(nameof(allowedChars));
            }

            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            var chars = ReplaceAllowedChars(str, allowedChars);

            // See https://msdn.microsoft.com/en-us/library/ms972966.aspx for info about \P key
            return
                Regex.IsMatch(chars, @"\P{IsBasicLatin}") &&
                Regex.IsMatch(chars, @"\P{IsLatin-1Supplement}") &&
                Regex.IsMatch(chars, @"\P{IsLatinExtended-A}") &&
                Regex.IsMatch(chars, @"\P{IsLatinExtended-B}") &&
                Regex.IsMatch(chars, @"\P{IsLatinExtendedAdditional}");
        }

        public static bool DoesConsistOfLetters(this string str)
        {
            return str.All(char.IsLetter);
        }

        public static bool DoesConsistOfNumbers(this string str)
        {
            return str.All(char.IsNumber);
        }

        public static bool IsFramedWith(this string str, string symbol, StringComparison stringComparison = StringComparison.InvariantCulture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return false;
            }

            return str.StartsWith(symbol, stringComparison) &&
                   str.EndsWith(symbol, stringComparison);
        }

        public static string LastLetterToLower(this string str)
        {
            return ConvertNthLetterCase(str, str.Length - 1, x => char.ToLower(x, CultureInfo.InvariantCulture));
        }

        public static string LastLetterToUpper(this string str)
        {
            return ConvertNthLetterCase(str, str.Length - 1, x => char.ToUpper(x, CultureInfo.InvariantCulture));
        }

        public static StringBuilder Substring(this StringBuilder sb, int start)
        {
            return sb.Substring(start, sb.Length - 1);
        }

        public static StringBuilder Substring(this StringBuilder sb, int start, int end)
        {
            if (start > sb.Length - 1 || end > sb.Length - 1 ||
                end < 0 || start < 0 ||
                end > start)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (start == end)
            {
                sb = new StringBuilder(sb[start]);
                return sb;
            }

            var newSb = new StringBuilder(sb.Length);

            for (int i = start; i < end; i++)
            {
                newSb.Append(sb[i]);
            }

            sb = newSb;
            return sb;
        }

        public static StringBuilder Trim(this StringBuilder sb)
        {
            return sb.Trim(true, true);
        }

        public static StringBuilder TrimEnd(this StringBuilder sb)
        {
            return sb.Trim(false, true);
        }

        public static StringBuilder TrimStart(this StringBuilder sb)
        {
            return sb.Trim(true, false);
        }

        private static string ConvertNthLetterCase(string str, int position, Func<char, char> convert)
        {
            if (convert == null)
            {
                throw new ArgumentNullException(nameof(convert));
            }

            if (str.Length == 0)
            {
                return str;
            }

            if (!char.IsLetter(str[0]))
            {
                return str;
            }

            var sb = new StringBuilder(str);
            sb[position] = convert.Invoke(sb[position]);

            return sb.ToString();
        }

        private static bool DoesNthCharacterUpper(string str, int position)
        {
            return !string.IsNullOrWhiteSpace(str) && char.IsUpper(str[position]);
        }

        private static string ReplaceAllowedChars(string str, char[] allowedChars)
        {
            return new string(Implementation().ToArray());

            IEnumerable<char> Implementation()
            {
                const char EmptyChar = new char();

                foreach (var character in str)
                {
                    if (allowedChars.Contains(character))
                    {
                        yield return EmptyChar;
                    }

                    yield return character;
                }
            }
        }

        public static bool Equals(this string lhs, string rhs, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            return string.Equals(lhs, rhs, stringComparison);
        }

        private static StringBuilder Trim(this StringBuilder sb, bool trimStart, bool trimEnd)
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }

            var firstNonWhiteSpaceCharPosition = 0;
            if (trimStart)
            {
                while (char.IsWhiteSpace(sb[firstNonWhiteSpaceCharPosition]))
                {
                    if (firstNonWhiteSpaceCharPosition >= sb.Length - 1)
                    {
                        sb.Clear();
                        return sb;
                    }

                    firstNonWhiteSpaceCharPosition++;
                }
            }

            var lastNonWhiteSpaceCharPosition = sb.Length - 1;
            if (trimEnd)
            {
                while (char.IsWhiteSpace(sb[lastNonWhiteSpaceCharPosition]))
                {
                    if (lastNonWhiteSpaceCharPosition <= 0)
                    {
                        sb.Clear();
                        return sb;
                    }

                    lastNonWhiteSpaceCharPosition--;
                }
            }

            return sb.Substring(firstNonWhiteSpaceCharPosition, lastNonWhiteSpaceCharPosition);
        }
    }
}