using System.Linq;

using Bitwise.Extensions;

using Xunit;

namespace Tests
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(0, new[] { false, })]
        [InlineData(1, new[] { true, })]
        [InlineData(2, new[] { true, false, })]
        [InlineData(7, new[] { true, true, true, })]
        [InlineData(15, new[] { true, true, true, true, })]
        [InlineData(16, new[] { true, false, false, false, false, })]
        [InlineData(511, new[] { true, true, true, true, true, true, true, true, true, })]
        [InlineData(512, new[] { true, false, false, false, false, false, false, false, false, false, })]
        [InlineData(513, new[] { true, false, false, false, false, false, false, false, false, true, })]
        [InlineData(200000001, new[] { true, false, true, true, true, true, true, false, true, false, true, true, true, true, false, false, false, false, true, false, false, false, false, false, false, false, false, true, })]
        [InlineData(4294967294, new[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, })]
        [InlineData(4294967295, new[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, })]
        public void GetNextBitMustWorkCorrectly(uint num, bool[] bits)
        {
            bool[] numbits = num.GetNextBit().ToArray();

            Assert.Equal<bool[]>(bits, numbits);
        }

        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, false)]
        [InlineData(4, true)]
        [InlineData(7, false)]
        [InlineData(8, true)]
        [InlineData(9, false)]
        [InlineData(127, false)]
        [InlineData(217, false)]
        [InlineData(219, false)]
        [InlineData(12587495, false)]
        [InlineData(4294967296, true)]
        [InlineData(4294907296, false)]
        [InlineData(3221225472, false)]
        public void IsPowerOfTwoMustWorkCorrectly(ulong v, bool result)
        {
            Assert.Equal<bool>(result, v.IsPowerOfTwo());
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        [InlineData(3, 4)]
        [InlineData(4, 8)]
        [InlineData(7, 8)]
        [InlineData(8, 16)]
        [InlineData(9, 16)]
        [InlineData(127, 128)]
        [InlineData(217, 256)]
        [InlineData(219, 256)]
        [InlineData(12587495, 16777216)]
        [InlineData(1073741824, 2147483648)]
        [InlineData(1073701824, 1073741824)]
        public void NextPowerOfTwoMustWorkCorrectly(ulong num, ulong result)
        {
            Assert.Equal<ulong>(result, num.GetNextPowerOfTwo());
        }
    }
}
