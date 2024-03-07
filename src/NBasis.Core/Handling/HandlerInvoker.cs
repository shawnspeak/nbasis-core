using NBasis.Commanding;
using NBasis.Querying;
using System.Reflection;

namespace NBasis.Handling
{
    internal class HandlerInvoker
    {        
        readonly Type _handlerType;
        readonly Type _inputType;
        readonly Type _outputType;

        public HandlerInvoker(Type inputType, Type outputType, Type handlerType)
        {
            _inputType = inputType;
            _outputType = outputType;
            _handlerType = handlerType;
        }

        public async Task<IHandlingContext<TInput, TOutput>> Invoke<TInput, TOutput>(IServiceProvider serviceProvider, TInput input, IDictionary<string, object> headers)
        {
            var handlingContext = CreateTheHandlingContext<TInput, TOutput>(input, headers);
            TOutput result = await InvokeTheHandlerAsync(serviceProvider, handlingContext);
            handlingContext.SetOutput(result);
            return handlingContext;
        }

        private async Task<TOutput> InvokeTheHandlerAsync<TInput, TOutput>(IServiceProvider serviceProvider, IHandlingContext<TInput, TOutput> handlingContext)
        {
            var handleMethod = GetTheHandleMethod();
            var handler = serviceProvider.GetService(_handlerType);

            try
            {
                return await (Task<TOutput>)handleMethod.Invoke(handler, new object[] { handlingContext });
            }
            catch (TargetInvocationException ex)
            {
                // bit of a hack to send the correct exception
                if (_inputType.GetInterface(nameof(ICommand)) != null)
                {
                    throw new CommandHandlerInvocationException<TInput, TOutput>(
                        string.Format("Command handler '{0}' for '{1}' failed. Inspect inner exception.", handler.GetType().Name, handlingContext.Input.GetType().Name),
                                                                      ex.InnerException)
                    {
                        HandlingContext = handlingContext
                    };
                } 
                else if (_inputType.GetInterface(nameof(IQuery<TOutput>)) != null)
                {
                    throw new QueryHandlerInvocationException<TInput, TOutput>(
                        string.Format("Query handler '{0}' for '{1}' failed. Inspect inner exception.", handler.GetType().Name, handlingContext.Input.GetType().Name),
                                                                                                                    ex.InnerException)
                    {
                        HandlingContext = handlingContext
                    };
                }
                else
                {
                    throw new HandlerInvocationException<TInput, TOutput>(
                        string.Format("Handler '{0}' for '{1}' failed. Inspect inner exception.", handler.GetType().Name, handlingContext.Input.GetType().Name),
                                                                                                                    ex.InnerException)
                    {
                        HandlingContext = handlingContext
                    };
                }
            }
        }

        private HandlingContext<TInput, TOutput> CreateTheHandlingContext<TInput, TOutput>(TInput input, IDictionary<string, object> headers)
        {
            var handlingContextType = typeof(HandlingContext<,>).MakeGenericType(_inputType, _outputType);
            return (HandlingContext<TInput, TOutput>)Activator.CreateInstance(handlingContextType, input, headers);
        }

        private MethodInfo GetTheHandleMethod()
        {
            return typeof(IHandler<,>).MakeGenericType(_inputType, _outputType).GetTypeInfo().GetDeclaredMethod("HandleAsync");
        }
    }
}
