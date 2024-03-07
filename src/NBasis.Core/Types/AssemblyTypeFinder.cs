using System.Diagnostics;
using System.Reflection;

namespace NBasis.Types
{
    /// <summary>
    /// Finds types in the given assemblies
    /// </summary>
    public class AssemblyTypeFinder : TypeFinder
    {
        public AssemblyTypeFinder(IEnumerable<Assembly> assemblies) : base(LoadTypes(assemblies))
        {
        }

        private static IEnumerable<Type> LoadTypes(IEnumerable<Assembly> assemblies)
        {
            var loadedTypes = new List<Type>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.ExportedTypes;
                    loadedTypes.AddRange(types);
                }
                catch (ReflectionTypeLoadException exception)
                {
                    exception.LoaderExceptions
                        .Select(e => e.Message)
                        .Distinct().ToList()
                        .ForEach(message => Debug.WriteLine(message));
                }
            }

            return loadedTypes;
        }
    }
}
