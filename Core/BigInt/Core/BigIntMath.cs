using System;

using BigInt.Entities;

using bigint = BigInt.Core.BigInt;

namespace BigInt.Core
{
    internal static class BigIntMath
    {
        public static bigint Subtract(bigint left, bigint right)
        {
            if (left == bigint.Zero)
            {
                return -right;
            }

            if (right == bigint.Zero)
            {
                return left;
            }

            return Implementation(left.Tail, right.Tail);

            bigint Implementation(BigIntBlock lhs, BigIntBlock rhs)
            {
                var result = new BigIntBlock();
                var currentBlock = result;

                while (!ReferenceEquals(lhs, null) && !ReferenceEquals(rhs, null))
                {
                    if (!(ReferenceEquals(lhs.NextDigit, null) && ReferenceEquals(rhs.NextDigit, null)))
                    {
                        if (ReferenceEquals(lhs.NextDigit, null))
                        {
                            lhs.NextDigit = bigint.Zero.Tail;
                        }

                        if (ReferenceEquals(rhs.NextDigit, null))
                        {
                            rhs.NextDigit = bigint.Zero.Tail;
                        }
                    }

                    if (currentBlock == null)
                    {
                        currentBlock = new BigIntBlock();
                    }

                    long leftValue = lhs.Digit.Value;
                    long rightValue = rhs.Digit.Value;

                    if (leftValue >= rightValue)
                    {
                        currentBlock.Digit = new Digit(leftValue - rightValue);
                    }
                    else
                    {
                        leftValue += uint.MaxValue;

                        var d = lhs.NextDigit.Digit;
                        d.Decrease();
                        lhs.NextDigit.Digit = d;

                        currentBlock.Digit = new Digit(leftValue - rightValue);
                    }

                    lhs = lhs.NextDigit;
                    rhs = rhs.NextDigit;
                    currentBlock = currentBlock.NextDigit;
                }

                return new bigint
                {
                    Tail = result,
                };
            }
        }

        public static bigint Add(bigint left, bigint right)
        {
            if (left == bigint.Zero)
            {
                return right;
            }

            if (right == bigint.Zero)
            {
                return left;
            }

            return Implementation(left.Tail, right.Tail);

            bigint Implementation(BigIntBlock lhs, BigIntBlock rhs)
            {
                var result = new BigIntBlock();
                var currentBlock = result;

                while (!ReferenceEquals(lhs, null) && !ReferenceEquals(rhs, null))
                {
                    if (!(ReferenceEquals(lhs.NextDigit, null) && ReferenceEquals(rhs.NextDigit, null)))
                    {
                        if (ReferenceEquals(lhs.NextDigit, null))
                        {
                            lhs.NextDigit = bigint.Zero.Tail;
                        }

                        if (ReferenceEquals(rhs.NextDigit, null))
                        {
                            rhs.NextDigit = bigint.Zero.Tail;
                        }
                    }

                    if (currentBlock == null)
                    {
                        currentBlock = new BigIntBlock();
                    }

                    long leftValue = lhs.Digit.Value;
                    if (left.Sign == Sign.Negative)
                    {
                        leftValue *= -1;
                    }

                    long rightValue = rhs.Digit.Value;
                    if (right.Sign == Sign.Negative)
                    {
                        rightValue *= -1;
                    }

                    long resultValue = leftValue + rightValue;

                    if (resultValue <= uint.MaxValue)
                    {
                        currentBlock.Digit = new Digit(resultValue);
                    }
                    else
                    {
                        resultValue -= uint.MaxValue;

                        var d = lhs.NextDigit.Digit;
                        d.Increase();
                        lhs.NextDigit.Digit = d;

                        currentBlock.Digit = new Digit(resultValue);
                    }

                    lhs = lhs.NextDigit;
                    rhs = rhs.NextDigit;
                    currentBlock = currentBlock.NextDigit;
                }

                return new bigint
                {
                    Tail = result,
                };
            }
        }

        public static (bigint quotient, bigint reminder) Divide(bigint dividend, bigint divisor)
        {
            if (dividend == bigint.Zero)
            {
                return (bigint.Zero, bigint.Zero);
            }

            if (divisor == bigint.Zero)
            {
                throw new DivideByZeroException();
            }


            if (dividend == bigint.One)
            {
                return (bigint.Zero, divisor);
            }

            if (divisor == bigint.One)
            {
                return (dividend, bigint.Zero);
            }

            return Implementation();

            (bigint quotient, bigint reminder) Implementation()
            {
                throw new NotImplementedException();
            }
        }

        public static bigint Multiple(bigint lhs, bigint rhs)
        {
            if (lhs == bigint.Zero || rhs == bigint.Zero)
            {
                return bigint.Zero;
            }

            if (lhs == bigint.One)
            {
                return rhs;
            }

            if (rhs == bigint.One)
            {
                return lhs;
            }

            return Implementation();

            bigint Implementation()
            {
                throw new NotImplementedException();
            }
        }
    }
}
