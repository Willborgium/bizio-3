using System;
using System.Collections.Generic;
using System.Linq;

namespace Bizio.App
{
    public static class RandomExtensions
    {
        public static T Random<T>(this Random r)
            where T : struct, Enum
        {
            var values = Enum.GetValues<T>();

            var index = _r.Next(values.Length);

            return values[index];
        }

        public static T Random<T>(this IEnumerable<T> collection)
        {
            var index = _r.Next(collection.Count());

            return collection.ElementAt(index);
        }

        public static T Random<T>(this IEnumerable<T> collection, Func<T, bool> condition)
        {
            var filtered = collection.Where(condition).ToList();

            var index = _r.Next(filtered.Count);

            return filtered[index];
        }

        public static int Random(this IntRange range)
        {
            var difference = range.Maximum - range.Minimum;
            return range.Minimum + _r.Next(difference + 1);
        }

        public static float Random(this FloatRange range)
        {
            var difference = range.Maximum - range.Minimum;
            return range.Minimum + (float)(_r.NextDouble() * difference);
        }

        private static readonly Random _r = new();
    }
}
