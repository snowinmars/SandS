using System;
using System.Collections.Generic;
using System.Linq;

using BitwiseOperations;

namespace Bitwise.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Return number as a bit collection.
        /// Collection starts from top order bit.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static IEnumerable<bool> GetNextBit(this uint num) => GetNextBit((ulong)num);

        /// <summary>
        /// Return number as a bit collection.
        /// Collection starts from top order bit.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static IEnumerable<bool> GetNextBit(this ulong num) => BitwiseOperation.GetBitsReversed(num).Reverse();

        /// <summary>
        /// Returns true if number is a power of two
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(this uint num) => IsPowerOfTwo((ulong)num);

        /// <summary>
        /// Returns true if number is a power of two
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(this bool[] bits) => bits.ToNumber().IsPowerOfTwo();

        /// <summary>
        /// Returns true if number is a power of two
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsPowerOfTwo(this ulong num) => num != 0 && (num & (num - 1)) == 0;

        /// <summary>
        /// Create new number from bit stream
        /// </summary>
        /// <param name="bits">Input stream</param>
        /// <returns></returns>
        public static ulong ToNumber(this bool[] bits)
        {
            if (bits == null)
            {
                return 0;
            }

            ulong divider = 1;
            ulong result = 0;

            foreach (bool bit in bits)
            {
                if (bit)
                {
                    result += divider;
                }

                divider *= 2;
            }

            return result;
        }

        /// <summary>
        /// Create new number from bit stream
        /// </summary>
        /// <returns></returns>
        public static bool[] ToBits(this uint num)
        {
            return ToBits((ulong)num);
        }

        /// <summary>
        /// Create new number from bit stream
        /// </summary>
        /// <returns></returns>
        public static bool[] ToBits(this ulong num)
        {
            var resultBits = new List<bool>();
            var length = Math.Floor(Math.Log10(num) + 1);
            if (length <= 0)
            {
                throw new Exception();
            }

            var currentPosition = 0;

            do
            {
                var currentBit = (int)(num % Math.Pow(10, length - currentPosition - 1));

                if (currentBit != 0 || currentBit != 1)
                {
                    throw new InvalidOperationException();
                }

                resultBits.Add(currentBit == 0);

                currentPosition++;
            }
            while (Math.Abs(currentPosition - length) > 0.0001);

            return resultBits.ToArray();
        }

        /// <summary>
        /// Compute next highest power of 2, e.g. for 114 it returns 128
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static ulong GetNextPowerOfTwo(this uint num)
        {
            return GetNextPowerOfTwo((ulong)num);
        }

        /// <summary>
        /// Compute next highest power of 2, e.g. for 114 it returns 128
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static ulong GetNextPowerOfTwo(this bool[] bits)
        {
            var num = bits.ToNumber();
            return num.GetNextPowerOfTwo();
        }

        /// <summary>
        /// Compute next highest power of 2, e.g. for 114 it returns 128
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static ulong GetNextPowerOfTwo(this ulong num)
        {
            if (num == 0)
            {
                return 1;
            }

            if (num.IsPowerOfTwo())
            {
                return num << 1;
            }

            num--;
            num |= num >> 1;
            num |= num >> 2;
            num |= num >> 4;
            num |= num >> 8;
            num |= num >> 16;
            num++;

            return num;
        }
    }
}
