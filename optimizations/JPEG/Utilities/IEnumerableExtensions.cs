using System;
using System.Collections.Generic;
using System.Linq;

namespace JPEG.Utilities
{
    static class IEnumerableExtensions
    {
        public static T MinOrDefault<T>(this IEnumerable<T> enumerable, Func<T, int> selector)
        {
            var valueKey = (Value: default(T), Key: int.MaxValue);
            foreach (var e in enumerable)
            {
                var key = selector(e);
                if (key < valueKey.Key)
                    valueKey = (e, key);
            }
            return valueKey.Value;
        }

        public static IEnumerable<T> Without<T>(this IEnumerable<T> enumerable, params T[] elements)
        {
            return enumerable.Where(x => !elements.Contains(x));
        }

        public static IEnumerable<T> ToEnumerable<T>(this T element)
        {
            yield return element;
        }
    }
}