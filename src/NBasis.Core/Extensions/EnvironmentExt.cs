using System;

namespace NBasis
{
    public class EnvironmentVariableMissingException : Exception
    {
        public EnvironmentVariableMissingException(string name) :
            base(string.Format("Missing environment variable: {0}", name))
        {
        }
    }

    public static class EnvironmentExt
    {
        public static void ThrowMissing(params string[] names)
        {
            if (names.SafeCount() == 0)
                throw new ArgumentNullException(nameof(names));

            foreach (var name in names)
                if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(name)))
                    throw new EnvironmentVariableMissingException(name);
        }
    }
}
