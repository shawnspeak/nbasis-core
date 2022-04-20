using NBasis.Types;
using System;

namespace NBasis.Container
{
    public class NContainer : IContainer
    {
        readonly IServiceProvider _serviceProvider;

        public NContainer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            // get type resolve cache
            var typeResolveCache = _serviceProvider.GetService(typeof(ITypeResolveCache)) as ITypeResolveCache;

            object ret = null;
            if (typeResolveCache != null)
                ret = typeResolveCache.Resolve(_serviceProvider, type);
            else
                ret = _serviceProvider.GetService(type);

            return ret;
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }
    }
}
