using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Utils
{
    internal static class IEnumerableExt
    {
        public static IEnumerable<IEnumerable<TValue>> Chunk<TValue>(
                     this IEnumerable<TValue> values,
                     int chunkSize)
        {
            using (var enumerator = values.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return GetChunk(enumerator, chunkSize).ToList();
                }
            }
        }

        private static IEnumerable<T> GetChunk<T>(
                         IEnumerator<T> enumerator,
                         int chunkSize)
        {
            do
            {
                yield return enumerator.Current;
            } while (--chunkSize > 0 && enumerator.MoveNext());
        }
    }
}
