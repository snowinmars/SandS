using Xunit;

namespace SandS.Common.Tests
{
    // ReSharper disable once AllowPublicClass
    public sealed class RandomizeTests
    {
        [Fact]
        public void Double()
        {
            Assert.Equal(0, Randomize.Double(0, 0));
            Assert.Equal(1, Randomize.Double(1, 1));

            Assert.True(Randomize.Double(1, 10) > 0, "1 10 > 0");
            Assert.True(Randomize.Double(10, 1) > 0, "10 1 > 0");
            Assert.True(Randomize.Double(-10, -1) < 0, "-10 -1 < 0");
            Assert.True(Randomize.Double(-1, -10) < 0, "-1 -10 < 0");

            Assert.True(Randomize.NegativeDouble(-17) < 0, "-17 < 0");
            Assert.True(Randomize.NegativeDouble(17) < 0, "17 < 0");

            Assert.True(Randomize.PositiveDouble(17) > 0, "17 > 0");
            Assert.True(Randomize.PositiveDouble(-17) > 0, "-17 > 0");
        }

        [Fact]
        public void Int()
        {
            Assert.Equal(0, Randomize.Int(0, 0));
            Assert.Equal(1, Randomize.Int(1, 1));

            Assert.True(Randomize.Int(1, 10) > 0, "1 10 > 0");
            Assert.True(Randomize.Int(10, 1) > 0, "10 1 > 0");
            Assert.True(Randomize.Int(-10, -1) < 0, "-10 -1 < 0");
            Assert.True(Randomize.Int(-1, -10) < 0, "-1 -10 < 0");

            Assert.True(Randomize.NegativeInt(-17) < 0, "-17 < 0");
            Assert.True(Randomize.NegativeInt(17) < 0, "17 < 0");

            Assert.True(Randomize.PositiveInt(17) > 0, "17 > 0");
            Assert.True(Randomize.PositiveInt(-17) > 0, "-17 > 0");
        }
    }
}