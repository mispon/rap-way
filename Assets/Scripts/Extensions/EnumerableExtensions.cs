using System;
using System.Collections.Generic;

namespace Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random random = new();

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }

            return list;
        }
    }
}