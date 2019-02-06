using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using big = BigInt.Core.BigInt;

namespace BigInt.Tests.BigIntMath
{
    public class Subtract : TestBase
    {
        [Theory]

        [InlineData(-17, 0, -17)]
        [InlineData(-1, 0, -1)]
        [InlineData(0, 0, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(17, 0, 17)]

        [InlineData(-17, -1, -16)]
        [InlineData(-1, -1, 0)]
        [InlineData(0, -1, 1)]
        [InlineData(1, -1, 2)]
        [InlineData(17, -1, 18)]
        public void IntegerMinusInteger(int lhsValue, int rhsValue, long resultValue)
        {
            var lhs = new big(lhsValue);
            var rhs = new big(rhsValue);

            var result = lhs - rhs;

            Assert.Null(result.Tail.NextDigit);
            Assert.Equal(resultValue, (int)result);
        }
    }
}
