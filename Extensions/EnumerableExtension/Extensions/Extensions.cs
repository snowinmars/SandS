using System;
using System.Collections.Generic;
using System.Linq;

namespace EnumerableExtension.Extensions
{
    public static class Extensions
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// Returns sequence of random elements from source.
        /// </summary>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.Shuffle(Random);
        }

        /// <summary>
        /// Returns sequence of random elements from source.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="rng">I need it due to I return random element of sequence. If you don't use this parameter, class will create it's own private static Random instance and will use it</param>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
        {
            var elements = source.ToArray();

            for (int i = elements.Length - 1; i > 0; i--)
            {
                int swapIndex = rng.Next(i + 1);
                yield return elements[swapIndex];
                elements[swapIndex] = elements[i];
            }

            yield return elements[0];
        }

        /// <summary>
        /// </summary>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        /// <returns></returns>
        public static bool SequenceEqualWithoutOrder<T>(this IEnumerable<T> source, IEnumerable<T> sequence)
            where T : IComparable<T>
        {
            return source.OrderBy(x => x).SequenceEqual(sequence.OrderBy(x => x));
        }

        /// <summary>
        /// If value doesn't equal to default value of that type, this function will return it. If it's not, this will return alternate
        /// </summary>
        public static T IfDefaultGiveMe<T>(this T value, T alternate)
        {
            return EqualityComparer<T>.Default.Equals(value, default) ? alternate : value;
        }

        /// <summary>
        /// If first item in this sequence is not null, this function will return it. If it's not, this will return alternate.
        /// </summary>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        public static T FirstOrAlternate<T>(this IEnumerable<T> source, T alternate)
        {
            return source.FirstOrDefault().IfDefaultGiveMe(alternate);
        }

        /// <summary>
        /// Function enumerate input collection and invoke func on every item of the collection
        /// </summary>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        public static IEnumerable<TOut> ForEach<TIn, TOut>(this IEnumerable<TIn> source, Func<TIn, TOut> func)
        {
            return source.Select(func.Invoke);
        }

        /// <summary>
        /// Function enumerate input collection and invoke func on every item of the collection
        /// </summary>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func), "Function is null");
            }

            foreach (T item in source)
            {
                func.Invoke(item);
                yield return item;
            }
        }

        /// <summary>
        /// This function do nothing with the collection, but it force IEnumerable to iterate
        /// </summary>
        /// <exception cref="ArgumentNullException">If any parameter is null</exception>
        public static void IterateEnumerator<T>(this IEnumerable<T> source)
        {
            using (var en = source.GetEnumerator())
            {
                while (en.MoveNext())
                { }
            }
        }
    }

    public static class IEnumerableExtensions
    {
        public static IEnumerable<T> Generate<T>(int count, Func<T> generate)
        {
            if (generate == null)
            {
                throw new ArgumentNullException(nameof(generate));
            }

            for (int i = 0; i < count; i++)
            {
                yield return generate.Invoke();
            }
        }
    }
}
