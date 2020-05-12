namespace NVShop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableExtensions
    {
        public static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var t in source)
            {
                action(t);
            }
        }

        public static string Join<T>(this IEnumerable<T> source, string separator, Func<T, string> expr)
        {
            return string.Join(separator, source.Select(expr));
        }

        public static string Join<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }
    }
}