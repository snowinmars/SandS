using System;
using System.Collections.Generic;
using System.Text;

namespace BigInt.Entities
{
    public class BigIntBlock : ICloneable<BigIntBlock>, IComparable<BigIntBlock>
    {
        public BigIntBlock() : this(0)
        {
        }

        public BigIntBlock(Digit digit)
        {
            this.Digit = digit;
        }

        public BigIntBlock(int value)
        {
            this.Digit = new Digit(value);
        }

        public BigIntBlock(uint value)
        {
            Digit = new Digit(value);
        }

        public BigIntBlock NextDigit { get; set; }

        public Digit Digit { get; set; }

        public BigIntBlock DeepClone()
        {
            var result = new BigIntBlock
            {
                Digit = this.Digit
            };

            if (this.NextDigit != null)
            {
                result.NextDigit = this.NextDigit.DeepClone();
            }

            return result;
        }

        public BigIntBlock ShallowClone()
        {
            return new BigIntBlock
            {
                Digit = this.Digit,
                NextDigit = this.NextDigit,
            };
        }

        public int CompareTo(BigIntBlock other)
        {
            const int ThisIsBigger = 1;
            const int OtherIsBigger = -1;
            const int Equals = 0;

            if (ReferenceEquals(this, null) && !ReferenceEquals(other, null))
            {
                if (DoesHaveAllLeadingZeros(other))
                {
                    return Equals;
                }

                return OtherIsBigger;
            }

            if (!ReferenceEquals(this, null) && ReferenceEquals(other, null))
            {
                if (DoesHaveAllLeadingZeros(this))
                {
                    return Equals;
                }

                return ThisIsBigger;
            }

            if (ReferenceEquals(this, null) && ReferenceEquals(other, null))
            {
                return Equals;
            }

            ////

            if (ReferenceEquals(this.NextDigit, null) && !ReferenceEquals(other.NextDigit, null))
            {
                if (DoesHaveAllLeadingZeros(other.NextDigit))
                {
                    return Equals;
                }

                return OtherIsBigger;
            }

            if (!ReferenceEquals(this.NextDigit, null) && ReferenceEquals(other.NextDigit, null))
            {
                if (DoesHaveAllLeadingZeros(this.NextDigit))
                {
                    return Equals;
                }

                return ThisIsBigger;
            }

            if (!ReferenceEquals(this.NextDigit, null) && !ReferenceEquals(other.NextDigit, null))
            {
                return this.NextDigit.CompareTo(other.NextDigit);
            }

            ////

            if (this.Digit.Value > other.Digit.Value)
            {
                return ThisIsBigger;
            }

            if (this.Digit.Value < other.Digit.Value)
            {
                return OtherIsBigger;
            }

            if (this.Digit.Value == other.Digit.Value)
            {
                return Equals;
            }

            throw new LogicPanicException();
        }

        public override string ToString()
        {
            var prefix = "";
            if (this.NextDigit != null)
            {
                prefix = "... ← ";
            }

            return $"{prefix}{this.Digit}";
        }

        private bool DoesHaveAllLeadingZeros(BigIntBlock block)
        {
            block = block.NextDigit;

            while (!ReferenceEquals(block, null) && !ReferenceEquals(block.NextDigit, null))
            {
                if (block.Digit.Value != 0)
                {
                    return false;
                }

                block = block.NextDigit;
            }

            return true;
        }
    }
}
