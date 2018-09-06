using System;
using System.Collections.Generic;

namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    public static class Zipper
    {
        public static IEnumerable<ZipperItem<T>> Zip<T>(Func<IEnumerable<T>> fetchA, Func<IEnumerable<T>> fetchB)
        {
            using (var enumeratorA = EnumerableExtensions.Items(fetchA).GetEnumerator())
            using (var enumeratorB = EnumerableExtensions.Items(fetchB).GetEnumerator())
            {
                bool hasCurrentA = enumeratorA.MoveNext();
                bool hasCurrentB = enumeratorB.MoveNext();

                while (hasCurrentA || hasCurrentB)
                {
                    bool useA, useB;

                    if (hasCurrentA && hasCurrentB)
                    {
                        var comparison = Comparer<T>.Default.Compare(enumeratorA.Current, enumeratorB.Current);
                        useA = comparison <= 0;
                        useB = comparison >= 0;
                    }
                    else if (hasCurrentA)
                    {
                        useA = true;
                        useB = false;
                    }
                    else
                    {
                        useA = false;
                        useB = true;
                    }

                    var item = new ZipperItem<T>(useA ? enumeratorA.Current : enumeratorB.Current, useA, useB);

                    if (useA)
                    {
                        hasCurrentA = enumeratorA.MoveNext();
                    }

                    if (useB)
                    {
                        hasCurrentB = enumeratorB.MoveNext();
                    }

                    yield return item;
                }
            }
        }
    }
}