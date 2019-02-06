using System;
using System.Text;

using BigInt.Entities;

using big = BigInt.Core.BigInt;

namespace BigInt.Core
{
    public struct BigInt : IComparable<BigInt>, ICloneable<BigInt>
    {
        internal BigIntBlock Tail { get; set; }

        public Sign Sign { get; set; }

        public static big NegativeOne => new big(-1);

        public static big One => new big(1);

        public static big Zero => new big(0);

        public BigInt(big bigInt)
        {
            this.Tail = bigInt.Tail.DeepClone();
            this.Sign = bigInt.Sign;
        }

        public BigInt(uint value, Sign sign)
        {
            this.Tail = new BigIntBlock(value);
            this.Sign = sign;
        }

        public BigInt(long value)
        {
            var abs = (ulong)Math.Abs(value);

            this.Tail = new BigIntBlock(0);
            var current = this.Tail;

            while (abs / int.MaxValue != 0)
            {
                current.Digit = current.Digit.Increase(int.MaxValue);
                current.NextDigit = new BigIntBlock();
                current = current.NextDigit;
                abs -= int.MaxValue;
            }

            current.Digit = current.Digit.Increase((int)abs % int.MaxValue);

            if (value == 0)
            {
                this.Sign = Sign.None;
            }
            else
            {
                this.Sign = value > 0 ? Sign.Positive : Sign.Negative;
            }
        }

        public BigInt(int value)
        {
            var abs = (uint)Math.Abs(value);

            this.Tail = new BigIntBlock(abs);

            if (value == 0)
            {
                this.Sign = Sign.None;
            }
            else
            {
                this.Sign = value > 0 ? Sign.Positive : Sign.Negative;
            }
        }

        #region operators

        #region unary

        public static big operator -(big bigInt)
        {
            var clone = bigInt.DeepClone();

            switch (bigInt.Sign)
            {
                case Sign.Unknown:
                    throw new LogicPanicException();

                case Sign.Positive:
                    clone.Sign = Sign.Negative;
                    break;

                case Sign.None:
                    break;

                case Sign.Negative:
                    clone.Sign = Sign.Positive;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return clone;
        }

        public static big operator +(big bigInt)
            => bigInt.DeepClone();

        public static big operator --(big bigInt)
            => BigIntMath.Subtract(bigInt, big.One);

        public static big operator ++(big bigInt)
            => BigIntMath.Add(bigInt, big.One);

        #endregion unary

        #region binary

        public static big operator +(big lhs, big rhs)
        {
            var isLeftPositive = lhs.Sign == Sign.Positive;
            var isRightPositive = rhs.Sign == Sign.Positive;

            // 5 + 3 = +(+5 + +3)
            if (isLeftPositive && isRightPositive)
            {
                return BigIntMath.Add(lhs, rhs);
            }

            // -5 + -3 = -5 - 3 = -(+5 + +3)
            if (!isLeftPositive && !isRightPositive)
            {
                return -BigIntMath.Add(-lhs, -rhs);
            }

            // +5 + -3 = +5 - +3 = 5 - 3 = +(+5 - +3)
            if (isLeftPositive)
            {
                return BigIntMath.Subtract(lhs, -rhs);
            }

            // -5 + +3 = +3 + -5 = 3 - 5 = +(+3 - +5)
            return BigIntMath.Subtract(-lhs, rhs);
        }

        public static big operator -(big lhs, big rhs)
        {
            var isLeftPositive = lhs.Sign == Sign.Positive;
            var isRightPositive = rhs.Sign == Sign.Positive;

            // +5 - +3 = +(+5 - +3)
            if (isLeftPositive && isRightPositive)
            {
                return BigIntMath.Subtract(lhs, rhs);
            }

            // -5 - -3 = -5 + 3 = +(-5 + +3)
            if (!isLeftPositive && !isRightPositive)
            {
                return BigIntMath.Add(lhs, -rhs);
            }

            // 5 - -3 = 5 + 3 = +(+5 + +3)
            if (isLeftPositive)
            {
                return BigIntMath.Add(lhs, -rhs);
            }

            // -5 - +3 = -5 - 3 = -(5 + 3) = -(+5 + +3)
            return -BigIntMath.Add(-lhs, rhs);
        }

        public static (BigInt quotient, BigInt reminder) operator /(BigInt dividend, BigInt divisor)
            => BigIntMath.Divide(dividend, divisor);

        public static big operator *(big lhs, big rhs)
            => BigIntMath.Multiple(lhs, rhs);

        #endregion binary

        #region eq

        public static bool operator ==(big lhs, big rhs)
        {
            return lhs.CompareTo(rhs) == 0;
        }

        public static bool operator !=(big lhs, big rhs)
            => !(lhs == rhs);

        public static bool operator <(big lhs, big rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <=(big lhs, big rhs)
            => lhs == rhs || lhs < rhs;

        public static bool operator >(big lhs, big rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator >=(big lhs, big rhs)
            => lhs == rhs || lhs > rhs;

        public int CompareTo(object obj)
        {
            if (!(obj is big bigInt))
            {
                return 1;
            }

            return this.CompareTo(bigInt);
        }

        public int CompareTo(big other)
        {
            if (this.Sign == Sign.Positive && other.Sign == Sign.Negative)
            {
                return 1;
            }

            if (this.Sign == Sign.Negative && other.Sign == Sign.Positive)
            {
                return -1;
            }

            return this.Tail.CompareTo(other.Tail);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is big bigInt))
            {
                return false;
            }

            return this.Equals(bigInt);
        }

        public bool Equals(big bigInt)
        {
            return this.CompareTo(bigInt) == 0;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        #endregion eq

        #endregion operators

        public big DeepClone()
        {
            var head = this.Tail;
            var result = new BigIntBlock();
            var current = result;

            while (head != null)
            {
                current.Digit = head.Digit;

                if (head.NextDigit!= null)
                {
                    current.NextDigit = new BigIntBlock();
                    current = current.NextDigit;
                }

                head = head.NextDigit;
            }

            return new big
            {
                Tail = result,
            };
        }

        public big ShallowClone()
        {
            return new big
            {
                Tail = this.Tail,
            };
        }

        //public override string ToString()
        //{
        //    var sb = new StringBuilder(32);

        //    var current = this;

        //    while (current != null)
        //    {
        //        int count = 0;
        //        byte a = 0;

        //        sb.Append("[ ");

        //        foreach (var item in BitwiseOperation.GetNextBit(current.Value.AbsoluteValue).Reverse())
        //        {
        //            sb.Append(item ? '1' : '0');

        //            if (a == 3)
        //            {
        //                sb.Append(' ');
        //                a = 0;
        //            }
        //            else
        //            {
        //                a++;
        //            }
        //            count++;
        //        }

        //        while (count < 32)
        //        {
        //            sb.Append('0');

        //            if (a == 3)
        //            {
        //                sb.Append(' ');
        //                a = 0;
        //            }
        //            else
        //            {
        //                a++;
        //            }

        //            count++;
        //        }

        //        sb.Append("] ");
        //        current = current.NextHeadBlock;
        //    }

        //    return sb.ToString();
        //}

        public static IBigIntParser GetParser()
        {
            return new BinaryBigIntParser();
        }

        public static string RightSubstring(string str, int length)
        {
            length = Math.Max(length, 0);

            return str.Length > length 
                ? str.Substring(str.Length - length, length) 
                : str;
        }

        public override string ToString()
        {
            string sign;

            switch (Sign)
            {
                case Sign.Unknown:
                    sign = "?";
                    break;

                case Sign.Positive:
                    sign = "+";
                    break;

                case Sign.None:
                    sign = "";
                    break;

                case Sign.Negative:
                    sign = "-";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return $"{sign} {this.Tail}";
        }

        public static explicit operator int(big value)
        {
            var abs = value.Tail.Digit.Value;

            if (value.Sign == Sign.Negative)
            {
                return -abs;
            }

            return abs;
        }
    }
}
