using BitwiseOperations;

using Xunit;

namespace Tests
{
    public class BitwiseOperationTests
    {
        [Theory]
        [InlineData(new[] { false, }, new[] { false, }, new[] { false, false, false, false })]
        [InlineData(new[] { false, }, new[] { true, }, new[] { false, false, false, false })]
        [InlineData(new[] { true, }, new[] { false, }, new[] { false, false, false, false })]
        [InlineData(new[] { true, }, new[] { true, }, new[] { false, false, false, true })]
        [InlineData(new[] { true, true, }, new[] { true, false, }, new[] { false, false, false, true, true, false, })]

        public void MultiplyMustWorkCorrectly(bool[] lhs, bool[] rhs, bool[] result)
        {
            bool[] output = BitwiseOperation.Multiply(lhs, rhs);

            Assert.Equal<bool[]>(result, output);
        }

        [Theory]
        [InlineData(new[] { false }, new[] { false }, new[] { false })]
        [InlineData(new[] { false }, new[] { true, }, new[] { true, })]
        [InlineData(new[] { true, }, new[] { false }, new[] { true, })]
        [InlineData(new[] { true, }, new[] { true, }, new[] { false })]
        [InlineData(new[] { true, true, false, false }, new[] { true, true, false, false }, new[] { true, false, false, false })]
        [InlineData(new[] { true, true, true, true, true, true, true, }, new[] { true, true, false, false, false, true, false }, new[] { true, true, false, false, false, false, true, })]

        public void AddMustWorkCorrectly(bool[] lhs, bool[] rhs, bool[] result)
        {
            bool[] output = BitwiseOperation.Add(lhs, rhs);

            Assert.Equal<bool[]>(result, output);
        }

        [Theory]
        [InlineData(new[] { false, }, new[] { false, }, new[] { false, })]
        [InlineData(new[] { true, }, new[] { false, }, new[] { true, })]
        [InlineData(new[] { false, }, new[] { true, }, new[] { true, })]
        [InlineData(new[] { true, }, new[] { true, }, new[] { false, })]
        [InlineData(new[] { true, true, false, false, }, new[] { false, true, true, false, }, new[] { false, true, true, false, })]
        [InlineData(new[] { true, false, false, false, false, false, false, true, }, new[] { false, false, false, true, false, false, false, true }, new[] { false, true, true, true, false, false, false, false })]
        public void SubtractMustWorkCorrectly(bool[] lhs, bool[] rhs, bool[] result)
        {
            bool[] output = BitwiseOperation.Subtract(lhs, rhs);

            Assert.Equal<bool[]>(result, output);
        }

        [Theory]
        [InlineData(new[] { false, }, new[] { false, }, 1)]
        [InlineData(new[] { true, }, new[] { true, }, 1)]
        [InlineData(new[] { true, true, }, new[] { true, true, }, 1)]
        [InlineData(new[] { true, true, false, false, true, false, true }, new[] { true, true, true, true, true, false, false }, 3)]
        public void ArithmeticRightShiftMustWorkCorrectly(bool[] num, bool[] bits, int shift)
        {
            bool[] output = BitwiseOperation.ArithmeticRightShift(num, shift);

            Assert.Equal<bool[]>(bits, output);
        }

        [Theory]
        [InlineData(new[] { false, }, new[] { true, })]
        [InlineData(new[] { true, }, new[] { false, })]
        [InlineData(new[] { true, true, }, new[] { false, false, })]
        [InlineData(new[] { true, true, false, true, false, true, true, true, false, true, true, false, false, true, true, false, true, false, false, false, true, false, true, true, true, false, true, true, false, true, true, }, new[] { false, false, true, false, true, false, false, false, true, false, false, true, true, false, false, true, false, true, true, true, false, true, false, false, false, true, false, false, true, false, false, })]
        [InlineData(new[] { true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, false, }, new[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, })]
        public void InvertMustWorkCorrectly(bool[] num, bool[] bits)
        {
            bool[] output = BitwiseOperation.Invert(num);

            Assert.Equal<bool[]>(bits, output);
        }

        [Theory]
        [InlineData(new[] { false, }, new[] { false, })]
        [InlineData(new[] { true, }, new[] { true, })]
        [InlineData(new[] { true, false, }, new[] { true, false, })]
        [InlineData(new[] { true, true, true, true, false, false, false, }, new[] { false, false, false, true, false, false, false, })]
        public void UnaryMinusMustWorkCorrectly(bool[] num, bool[] result)
        {
            bool[] output = BitwiseOperation.UnaryMinus(num);

            Assert.Equal<bool[]>(result, output);
        }
    }
}
