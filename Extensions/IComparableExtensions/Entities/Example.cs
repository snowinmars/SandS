using System;
using System.Collections;
using System.Collections.Generic;

namespace Entities
{
    public class Example : IComparable,
        IComparable<Example>,
        IEquatable<Example>
    {
        public int Value { get; set; }

// insert inside an entity class
#region comparer
        private static readonly ExampleComparer LocalComparer = new ExampleComparer();

        public IComparer<Example> Comparer => LocalComparer;

        public IEqualityComparer<Example> EqualityComparer => LocalComparer;

        private sealed class ExampleComparer : IComparer,
                                       IComparer<Example>,
                                       IEqualityComparer,
                                       IEqualityComparer<Example>
        {
            public int Compare(Example lhs, Example rhs)
            {
                const int equals = 0;
                const int notEquals = 1; // introduce less/greater constants if you need them

                if (lhs is null &&
                    rhs is null)
                {
                    return equals;
                }

                if (lhs is null ||
                    rhs is null)
                {
                    return notEquals;
                }

                if (ReferenceEquals(lhs, rhs))
                {
                    return equals;
                }

                var areEquals = lhs.Value == rhs.Value; /* compare here */

                return areEquals ? equals : notEquals;
            }

            public int Compare(object lhs, object rhs)
            {
                const int equals = 0;
                const int notEquals = 1;

                if (lhs is null &&
                    rhs is null)
                {
                    return equals;
                }

                if (lhs is null ||
                    rhs is null)
                {
                    return notEquals;
                }

                if (ReferenceEquals(lhs, rhs))
                {
                    return equals;
                }

                if (lhs is Example lhsItem &&
                    rhs is Example rhsItem)
                {
                    return Compare(lhsItem, rhsItem);
                }

                return notEquals;
            }

            public bool Equals(Example lhs, Example rhs)
            {
                return Compare(lhs, rhs) == 0;
            }

            bool IEqualityComparer.Equals(object lhs, object rhs)
            {
                return Compare(lhs, rhs) == 0;
            }

            public int GetHashCode(Example item)
            {
                unchecked
                {
                    unchecked
                    {
                        // Ensure that there are no warnings here
                        // Ensure that there are no mutable fields in hash code calculations
                        return item.Value.GetHashCode(); /* calculate here */
                    }
                }
            }

            public int GetHashCode(object obj)
            {
                if (obj is Example item)
                {
                    return GetHashCode(item);
                }

                return obj.GetHashCode();
            }
        }

        public int CompareTo(object obj)
        {
            return LocalComparer.Compare(this, obj);
        }

        public int CompareTo(Example other)
        {
            return LocalComparer.Compare(this, other);
        }

        public override bool Equals(object obj)
        {
            return LocalComparer.Compare(this, obj) == 0;
        }

        public bool Equals(Example other)
        {
            return LocalComparer.Compare(this, other) == 0;
        }

        public override int GetHashCode()
        {
            return EqualityComparer.GetHashCode(this);
        }

        public static bool operator ==(Example lhs, Example rhs)
        {
            return LocalComparer.Compare(lhs, rhs) == 0;
        }

        public static bool operator !=(Example lhs, Example rhs)
        {
            return LocalComparer.Compare(lhs, rhs) != 0;
        }
#endregion
    }
}
