namespace NBasis
{
    public static class NullExtensions
    {
        /// <summary>
        /// Returns the original value of a nullable type if it is not null, or the alternate if it is.
        /// </summary>
        /// <typeparam name="T">Type of the value to compare.</typeparam>
        /// <param name="original">Original value.</param>
        /// <param name="alternate">Alternate value.</param>
        /// <returns>Original value if it is not null, or the alternate value otherwise.</returns>
        public static T Or<T>(this T? original, T alternate) where T : struct
        {
            return original == null ? alternate : original.Value;
        }

        public static T Or<T>(this T original, T alternate)
        {
            return null == original || original.Equals(default(T)) || (original is string && string.IsNullOrWhiteSpace(original.ToString()))
                ? alternate
                : original;
        }

        public static T Value<T>(this bool condition, T value)
        {
            return condition ? value : default;
        }

        public static TValue Value<TObject, TValue>(this TObject context, Func<TObject, TValue> fn)
        {
            return context is not null ? fn(context) : default;
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> list)
        {
            return list ?? Enumerable.Empty<T>();
        }
    }
}
