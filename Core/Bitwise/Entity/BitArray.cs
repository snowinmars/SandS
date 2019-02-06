using System;

using Bitwise.Extensions;

using BitwiseOperations;

namespace Entity
{
    public class BitArray
    {
        private bool[] bits;

        public BitArray(ulong bitwiseValue) : this(bitwiseValue.ToBits())
        {
        }

        public BitArray(bool[] bits)
        {
            this.bits = bits;
        }

        public static BitArray operator +(BitArray lhs, BitArray rhs)
        {
            var num = BitwiseOperation.Add(lhs.bits, rhs.bits);
            return new BitArray(num);
        }

        public static BitArray operator -(BitArray lhs, BitArray rhs)
        {
            var num = BitwiseOperation.Subtract(lhs.bits, rhs.bits);
            return new BitArray(num);
        }

        public static BitArray operator *(BitArray lhs, BitArray rhs)
        {
            var num = BitwiseOperation.Multiply(lhs.bits, rhs.bits);
            return new BitArray(num);
        }

        public static BitArray operator ~(BitArray bits)
        {
            var num = BitwiseOperation.Invert(bits.bits);
            return new BitArray(num);
        }

        public static BitArray operator -(BitArray bits)
        {
            var num = BitwiseOperation.UnaryMinus(bits.bits);
            return new BitArray(num);
        }

        public static BitArray operator >>(BitArray bits, int shift)
        {
            var num = BitwiseOperation.ArithmeticRightShift(bits.bits, shift);
            return new BitArray(num);
        }
    }
}
