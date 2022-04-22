namespace NBasis
{
    public static class DictionaryExtensions
    {
        public static void CopyTo<TKey, TValue>(this IDictionary<TKey, TValue> from, IDictionary<TKey, TValue> to)
        {
            if ((from == null) || (from.Count == 0) || (to == null)) return;
            from.Keys.ForEach((key) =>
            {
                to[key] = from[key];
            });
        }

        /// <summary>
        /// Checks to see if dictionary contains key and safely returns the value OR null depending upon existence
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns>The value if found or null if not</returns>
        /// <remarks>It's possible to store a null value in the dictionary and this will not be detected upon return</remarks>
        public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            if (dictionary == null || dictionary.SafeCount() == 0)
                return null;
            if (!dictionary.ContainsKey(key))
                return null;
            return dictionary[key];
        }
    }
}
