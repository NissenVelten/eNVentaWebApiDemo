namespace NVShop
{
    using System.Collections.Generic;

    public static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return InnerBatch(enumerator, batchSize - 1);
                }
            }
        }

        private static IEnumerable<T> InnerBatch<T>(IEnumerator<T> source, int batchSize)
        {
            yield return source.Current;
            for (var i = 0; (i < batchSize) && source.MoveNext(); i++)
            {
                yield return source.Current;
            }
        }
    }
}