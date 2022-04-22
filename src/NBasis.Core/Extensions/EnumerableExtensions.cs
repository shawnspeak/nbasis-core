﻿namespace NBasis
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Yield a single object as IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="one"></param>
        /// <returns></returns>
        public static IEnumerable<T> Yield<T>(this T one)
        {
            yield return one;
        }

        /// <summary>
        /// Foreach helper for IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if ((items == null) || (action == null)) return; // do nothing
            foreach (var item in items)
                action(item);
        }

        /// <summary>
        /// Count down helper
        /// </summary>
        /// <param name="count"></param>
        /// <param name="each"></param>
        public static void CountDown(this int count, Action<int> each)
        {
            if (each == null) return; // do nothing
            for (int i = count; i > 0; i--)
                each(i - 1);
        }

        /// <summary>
        /// Count up helper
        /// </summary>
        /// <param name="count"></param>
        /// <param name="each"></param>
        public static void CountUp(this int count, Action<int> each)
        {
            if (each == null) return; // do nothing
            for (int i = 0; i < count; i++)
                each(i);
        }

        /// <summary>
        /// Paged enumerable results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="action"></param>
        /// <param name="pageSize"></param>
        public static void PagedForEach<T>(this IEnumerable<T> items, Action<T> action, int pageSize = 50)
        {
            // paged query
            int skip = 0;
            int read = items.Count();
            while (read > 0)
            {
                // take a page
                var page = items.Skip(skip).Take(pageSize).ToList();
                read = page.Count;
                skip += read;

                if (read > 0)
                {
                    page.ForEach(action);
                }

                if (read < pageSize)
                    break; // we are done
            }
        }

        public static IEnumerable<T> BuildUp<T>(this int count, Func<int, T> map)
        {
            if (map != null)
                for (int i = 0; i < count; i++)
                    yield return map(i);
        }

        public static int FindIndex<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            int i = 0;
            foreach (var item in list)
            {
                if (predicate(item)) { return i; }
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Chunk an enumerable into smaller fixed size chunks
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> items, int chunkSize)
        {
            if (chunkSize < 2)
                throw new ArgumentOutOfRangeException(nameof(chunkSize), "chunkSize must be greater than 1");

            int skip = 0;
            int read = items.SafeCount();
            while (read > 0)
            {
                // take a page
                var page = items.Skip(skip).Take(chunkSize).ToList();
                read = page.Count;
                skip += read;
                if (read > 0)
                    yield return page;

                if (read < chunkSize)
                    break; // we are done
            }
        }
    }
}
