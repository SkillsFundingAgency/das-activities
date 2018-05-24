using System;
using System.Collections.Generic;

namespace SFA.DAS.Activities.IntegrityChecker.Utils
{
    static class EnumerableExtensions
    {
        /// <summary>
        ///     Converts a function that returns batches of items to a straight IEnumerable. This allows 
        ///     a paging function say to be enumerated over as though it were a single enumeration.
        /// </summary>
        /// <typeparam name="T">The type of the enumeration</typeparam>
        /// <param name="fetcher">
        ///     A delegate for fetching the next page of items. The numeration terminates when the function
        ///     returns null or an empty enumeration.
        /// </param>
        /// <returns>
        ///     An IEnumerable of T that enumerates all the items returned by the pager function.
        /// </returns>
        /// <remarks>
        ///     It is not possible for this to an async method (to allow <see cref="fetcher"/> to be async)
        ///     because yield and await cannot be used together (await can only return a Task and a task 
        ///     is not enumerable as required by yield). So fetcher will probably be blocking. 
        /// </remarks>
        public static IEnumerable<T> Items<T>(Func<IEnumerable<T>> fetcher)
        {
            IEnumerable<T> batch;
            bool isEmptyBatch = false;

            while (!isEmptyBatch && ((batch = fetcher()) != null))
            {
                isEmptyBatch = true;
                foreach (var item in batch)
                {
                    isEmptyBatch = false;
                    yield return item;
                }
            }
        }
    }
}