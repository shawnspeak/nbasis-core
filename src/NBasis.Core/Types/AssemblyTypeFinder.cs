using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace NBasis.Types
{
    public class AssemblyTypeFinder : ITypeFinder
    {
        readonly IEnumerable<Type> _allTypes;

        public AssemblyTypeFinder(IEnumerable<Assembly> assemblies)
        {
            _allTypes = LoadTypes(assemblies);
        }

        public IEnumerable<Type> AllLoadedTypes
        {
            get { return _allTypes; }
        }

        public IEnumerable<Type> GetDerivedTypes<TBase>() where TBase : class
        {
            var baseType = typeof(TBase);

            IEnumerable<Type> derivedTypes()
            {
                foreach (var derivedType in _allTypes)
                {
                    if (baseType != derivedType)
                    {
                        if (baseType.GetTypeInfo().IsAssignableFrom(derivedType.GetTypeInfo()))
                        {
                            yield return derivedType;
                        }
                    }
                }
            }

            return derivedTypes().ToArray();
        }

        public IEnumerable<Type> GetInterfaceImplementations<TInterface>() where TInterface : class
        {
            IEnumerable<Type> derivedTypes()
            {
                foreach (var derivedType in _allTypes)
                {
                    if (!derivedType.GetTypeInfo().IsInterface)
                    {
                        foreach (var interfaceType in derivedType.GetTypeInfo().ImplementedInterfaces)
                        {
                            if (interfaceType == typeof(TInterface))
                            {
                                yield return derivedType;
                            }
                        }
                    }
                }
            }
            return derivedTypes().Distinct().ToArray();
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
