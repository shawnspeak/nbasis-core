using Microsoft.Extensions.Configuration;

namespace NBasis
{
    public static class ConfigurationExtensions
    {
        public static string Get(this IConfiguration configuration, string key)
        {
            return Get(configuration, key, null);
        }

        public static string Get(this IConfiguration configuration, string key, string defaultValue)
        {
            if (configuration == null) return defaultValue;
            try
            {
                return configuration[key] ?? defaultValue;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static int GetInt(this IConfiguration configuration, string key, int defaultValue = 0)
        {
            return Int32.TryParse(Get(configuration, key), out int val) ? val : defaultValue;
        }

        static readonly string[] _truths = new string[] { "true", "1", "yes" };

        public static bool GetBool(this IConfiguration configuration, string key, bool defaultValue)
        {
            return _truths.Contains(Get(configuration, key, defaultValue.ToString()).ToLower());
        }
    }
}
