using System;

namespace BigInt.Entities
{
    public struct Digit : IComparable, IComparable<Digit>
    {
        public Digit(long value)
        {
            long abs = Math.Abs(value);

            if (abs > uint.MaxValue || abs < uint.MinValue)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.Value = (int)value;
        }

        public int Value { get; private set; }

        public Digit Increase() => Increase(1);

        public Digit Increase(int delta)
        {
            this.Value += delta;

            return this;
        }

        public Digit Decrease() => Decrease(1);

        public Digit Decrease(int delta)
        {
            this.Value -= delta;

            return this;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Digit digit && this.Equals(digit);
        }

        public bool Equals(Digit other)
        {
            return this.Value.CompareTo(other.Value) == 0;
        }

        public override string ToString()
        {
            return $"{this.Value}";
        }

        public int CompareTo(object obj)
        {
            if (obj is Digit digit)
            {
                return this.CompareTo(digit);
            }

            return 1;
        }

        public int CompareTo(Digit other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}
