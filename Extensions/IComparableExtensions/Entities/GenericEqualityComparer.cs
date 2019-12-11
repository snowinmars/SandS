using System;
using System.Collections.Generic;

namespace Entities
{
    public sealed class GenericEqualityComparer<TItem, TProperty>
        : IEqualityComparer<TItem>, IComparer<TItem>
        where TItem : class
    {
        public GenericEqualityComparer(Func<TItem, TProperty> property)
        {
            equals = (x, y) => EqualityComparer<TProperty>.Default.Equals(property(x), property(y));
            getHashCode = x => EqualityComparer<TProperty>.Default.GetHashCode(property(x));
        }

        private readonly Func<TItem, TItem, bool> equals;

        private readonly Func<TItem, int> getHashCode;

        public int Compare(TItem x, TItem y)
        {
            return equals(x, y) ? 0 : 1;
        }

        public bool Equals(TItem lhs, TItem rhs)
        {
            return equals(lhs, rhs);
        }

        public int GetHashCode(TItem obj)
        {
            return getHashCode(obj);
        }
    }
}
