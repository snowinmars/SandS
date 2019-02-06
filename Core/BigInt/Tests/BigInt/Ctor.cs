using System;
using Xunit;

using big = BigInt.Core.BigInt;

namespace BigInt.Tests
{
    public class Ctor : TestBase
    {
        [Fact]
        public void Empty()
        {
            var something = new big();

            Assert.Equal(big.Zero, something);
        }

        [Theory]
        [InlineData(-17)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(17)]
        public void Integer(int value)
        {
            var something = new big(value);

            Assert.Equal(value, (int)something);
        }

        [Theory]
        [InlineData(-17)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(17)]
        public void BigInt(int value)
        {
            var something = new big(value);
            var another = new big(something);

            Assert.Equal(something, another);
        }
    }
}
