using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BigInt.Entities;

using big = BigInt.Core.BigInt;

namespace BigInt.Core
{
    public class BinaryBigIntParser : IBigIntParser
    {
        private static readonly char[] AllowedSeparators =
        {
            ' ',
            '\r',
            '\n',
            '_',
            '.',
            ',',
        };

        public big Parse(string str)
        {
            var result = new big(0);
            var current = result.Tail;
            var currentDigit = -1;

            for (int i = str.Length - 1; i >= 0; i--)
            {
                var symbol = str[i];

                if (AllowedSeparators.Contains(symbol))
                {
                    continue;
                }

                currentDigit++;

                // create new block on int overflow
                const int BitsInInt32 = 32;
                if (currentDigit != 0 && currentDigit % BitsInInt32 == 0)
                {
                    current.NextDigit = new BigIntBlock();
                    current = current.NextDigit;
                }

                switch (symbol)
                {
                    case '1':
                        current.Digit = current.Digit.Increase((int)Math.Pow(2, currentDigit % BitsInInt32));
                        continue;

                    case '0':
                        continue;

                    default:
                        throw new FormatException();
                }
            }

            return result;
        }
    }
}
