using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Other
{
    public class SequenceComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return
                x is IEnumerable enumerableX &&
                y is IEnumerable enumerableY &&
                enumerableX.Cast<object>().SequenceEqual(enumerableY.Cast<object>());
        }

        public int GetHashCode(T obj)
        {
            return obj is IEnumerable enumerable
                    ? enumerable.Cast<object>()
                        .Select(e => e.GetHashCode())
                        .Aggregate(17, (a, b) => 23 * a + b)
                    : obj.GetHashCode();
        }
    }
}