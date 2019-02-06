using System;
using System.Collections.Generic;
using System.Text;

using big = BigInt.Core.BigInt;
using Xunit;

namespace BigInt.Tests.Parsers.Binary
{
    public class BinaryBigIntParser : TestBase
    {
        [Theory]

        [InlineData("0", 0)]
        [InlineData("1", 1)]
        [InlineData("01", 1)]
        [InlineData("0000_0001", 1)]
        [InlineData("00000001", 1)]
        [InlineData("10", 2)]
        [InlineData("11", 3)]
        [InlineData("0111 1010 1011 0111", 31415)]
        [InlineData(@"0111 1111 1111 1111 
                      1111 1111 1111 1111", 2147483647)]
        [InlineData(@"1000 0000 0000 0000 
                      0000 0000 0000 0000", 2147483648)]
        //[InlineData(@"1001 1010 1000 0100
        //                0011 0100 1110 1100
        //                    1000 1110 0010 0010 0101", 2718281828459045)]
        public void BinaryParseWithoutIntOverflow(string str, long expectedResult)
        {
            var bigint = big.GetParser().Parse(str);
            var result = new big(expectedResult);

            Assert.Equal(result, bigint);
        }
    }
}
