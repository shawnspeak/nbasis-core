using NBasis.Handling;
using NBasis.Types;

namespace NBasis.Commanding
{
    public class LocalCommanderFactory : ICommanderFactory
    {
        readonly InvokerResolver<IHandleCommands> _resolver;

        public LocalCommanderFactory(ITypeFinder typeFinder)
        {
            _resolver = new InvokerResolver<IHandleCommands>(typeFinder);
        }

        public ICommander GetCommander(IServiceProvider serviceProvider, CommandOptions options)
        {
            return new Commander(serviceProvider, options, _resolver);
        }

        private class Commander : ICommander
        {
            readonly IServiceProvider _serviceProvider;
            readonly InvokerResolver<IHandleCommands> _resolver;
            readonly CommandOptions _options;

            private int _commandLevel = 0;

            public Commander(IServiceProvider serviceProvider, CommandOptions options, InvokerResolver<IHandleCommands> resolver)
            {
                _serviceProvider = serviceProvider;
                _resolver = resolver;
                _options = options;
            }         

            public async Task<TResult> AskAsync<TResult>(Envelope<ICommand<TResult>> command)
            {
                TResult result = default;
                try
                {
                    var body = CommandCleaner.Clean(command.Body);

                    var handler = _resolver.GetTheHandler(body.GetType()) ?? throw new CommandHandlerNotFoundException(body.GetType());

                    var context = await handler.Invoke<ICommand<TResult>, TResult>(_serviceProvider, body, command.Headers);

                    result = context.Output;

                    if (_options?.OnSuccess != null)
                    {
                        // keep track of command level
                        _commandLevel++;
                        await _options.OnSuccess(new OptionContext(_commandLevel));
                    }
                }
                catch (Exception ex)
                {
                    if (_options?.OnException != null)
                    {
                        bool continueThrow = await _options.OnException(ex);
                        if (continueThrow) throw;
                    }
                    else
                        throw;
                }

                return result;
            }
        }
    }
}
