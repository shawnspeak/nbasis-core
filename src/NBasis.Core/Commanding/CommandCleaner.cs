using System.Reflection;

namespace NBasis.Commanding
{
    internal class CommandCleaner
    {
        public static ICommand Clean(ICommand input)
        {
            // look for no clean
            var flags = CommandCleaner.GetFlags(input);
            if (flags == CleanupFlags.None) return null;

            // cleanup each property with flags
            var props = input.GetType().GetTypeInfo().DeclaredProperties;
            foreach (var prop in props)
            {
                // only care about strings for the moment
                if (prop.PropertyType == typeof(string))
                {
                    CleanupFlags propFlags = flags;

                    // is there a attribute override?
                    var attrs = prop.GetCustomAttributes(typeof(CommandCleanupAttribute), false) as CommandCleanupAttribute[];
                    if (attrs.SafeCount() > 0)
                    {
                        var attr = attrs.FirstOrDefault();
                        if (attr != null)
                            propFlags = attr.Levels;
                    }

                    if (propFlags == CleanupFlags.None) continue;

                    if (prop.CanWrite)
                    {
                        // get the value
                        if (prop.GetValue(input) is string val)
                        {
                            if ((propFlags & CleanupFlags.TrimString) > 0)
                                val = val.Trim();
                            if ((propFlags & CleanupFlags.LowerString) > 0)
                                val = val.ToLower();

                            prop.SetValue(input, val);
                        }
                    }
                }
            }

            return input;
        }

        private static CleanupFlags GetFlags(ICommand input)
        {
            // nothing to clean
            if (input == null) return CleanupFlags.None;

            // get attribute and collect levels
            CleanupFlags collect = CleanupFlags.TrimString;
            var attrs = input.GetType().GetTypeInfo().GetCustomAttributes(typeof(CommandCleanupAttribute), true) as CommandCleanupAttribute[];
            if (attrs.SafeCount() > 0)
            {
                // look for none
                foreach (var attr in attrs)
                    if (attr.Levels == CleanupFlags.None)
                        return CleanupFlags.None;
                    else
                        collect |= attr.Levels;
            }

            // nothing stopping us
            return collect;
        }
    }
}
