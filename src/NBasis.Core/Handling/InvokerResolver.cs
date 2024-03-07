using NBasis.Types;
using System.Reflection;

namespace NBasis.Handling
{
    internal class InvokerResolver<THandler> where THandler : class
    {
        private IDictionary<Type, HandlerInvoker> _invokers;

#pragma warning disable IDE0044 // Add readonly modifier
        private static object _lock = new();
#pragma warning restore IDE0044 // Add readonly modifier

        public InvokerResolver(ITypeFinder typeFinder)
        {
            BuildInvokers(typeFinder);
        }

        private void BuildInvokers(ITypeFinder typeFinder)
        {
            if (_invokers == null)
            {
                lock (_lock)
                {
                    if (_invokers == null)
                    {
                        var invokers = new Dictionary<Type, HandlerInvoker>();

                        // get all of the handlers the typefinder knows about
                        foreach (var handlerType in typeFinder.GetInterfaceImplementations<THandler>())
                        {
                            foreach (var inputType in GetInputTypesForHandler(handlerType))
                            {
                                if (invokers.ContainsKey(inputType))
                                    throw new DuplicateHandlersException(inputType);

                                var outputType = GetOutputTypeForHandler(inputType);
                                invokers.Add(inputType, new HandlerInvoker(inputType, outputType, handlerType));
                            }
                        }
                        _invokers = invokers;
                    }
                }
            }
        }

        private static IEnumerable<Type> GetInputTypesForHandler(Type handlerType)
        {
            return (from interfaceType in handlerType.GetTypeInfo().ImplementedInterfaces
                    where interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandler<,>)
                    select interfaceType.GetTypeInfo().GenericTypeArguments[0]).ToArray();
        }

        private static Type GetOutputTypeForHandler(Type handlerType)
        {
            return (from interfaceType in handlerType.GetTypeInfo().ImplementedInterfaces
                    where interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandler<,>)
                    select interfaceType.GetTypeInfo().GenericTypeArguments[1]).First();
        }

        internal HandlerInvoker GetTheHandler(Type input)
        {
            if (!_invokers.TryGetValue(input, out HandlerInvoker invoker))
                return null;
            return invoker;
        }
    }
}
