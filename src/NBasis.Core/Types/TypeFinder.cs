using System.Reflection;

namespace NBasis.Types
{
    public class TypeFinder : ITypeFinder
    {
        // a list of types the finder will search through
        protected IEnumerable<Type> _allTypes;

        public TypeFinder(IEnumerable<Type> types)
        {
            _allTypes = types;
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
    }
}
