using NBasis.Types;
using System.Reflection;

namespace NBasis.Commanding
{
    public class LocalCommandBusFactory : ICommandBusFactory
    {
        private IDictionary<Type, CommandHandlerInvoker> commandInvokers;
#pragma warning disable IDE0044 // Add readonly modifier
        private static object _lock = new();
#pragma warning restore IDE0044 // Add readonly modifier

        public LocalCommandBusFactory(ITypeFinder typeFinder)
        {
            BuildCommandInvokers(typeFinder);
        }

        public ICommandBus GetCommandBus(IServiceProvider serviceProvider, CommandOptions options)
        {
            return new CommandBus(serviceProvider, options, this);
        }

        private class CommandBus : ICommandBus
        {
            readonly IServiceProvider _serviceProvider;
            readonly LocalCommandBusFactory _factory;
            readonly CommandOptions _options;

            private int _commandLevel = 0;

            public CommandBus(IServiceProvider serviceProvider, CommandOptions options, LocalCommandBusFactory factory)
            {
                _serviceProvider = serviceProvider;
                _factory = factory;
                _options = options;
            }

            public async Task SendAsync(IEnumerable<Envelope<ICommand>> commands)
            {
                try
                {
                    List<Task> tasks = new();
                    commands.ForEach((command) =>
                    {
                        var body = CommandCleaner.Clean(command.Body);

                        var commandHandler = _factory.GetTheCommandHandler(body);
                        if (commandHandler == null) return;

                        tasks.Add(commandHandler.SendAsync(_serviceProvider, body, command.Headers, command.CorrelationId));
                    });
                    await Task.WhenAll(tasks);

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
            }

            public async Task<TResult> AskAsync<TResult>(Envelope<ICommand> command)
            {
                TResult result = default;
                try
                {
                    var body = CommandCleaner.Clean(command.Body);
                    var commandHandler = _factory.GetTheCommandHandler(body);
                    if (commandHandler == null) return default;
                    result = await commandHandler.AskAsync<TResult>(_serviceProvider, body, command.Headers, command.CorrelationId);

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

        private void BuildCommandInvokers(ITypeFinder typeFinder)
        {
            if (commandInvokers == null)
            {
                lock (_lock)
                {
                    if (commandInvokers == null)
                    {
                        commandInvokers = new Dictionary<Type, CommandHandlerInvoker>();
                        foreach (var commandHandlerType in typeFinder.GetInterfaceImplementations<IHandleCommands>())
                        {
                            foreach (var commandType in GetCommandTypesForCommandHandler(commandHandlerType))
                            {
                                if (commandInvokers.ContainsKey(commandType))
                                    throw new DuplicateCommandHandlersException(commandType);

                                commandInvokers.Add(commandType, new CommandHandlerInvoker(commandType, commandHandlerType));
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Type> GetCommandTypesForCommandHandler(Type commandHandlerType)
        {
            return (from interfaceType in commandHandlerType.GetTypeInfo().ImplementedInterfaces
                    where interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IHandleCommands<>)
                    select interfaceType.GetTypeInfo().GenericTypeArguments[0]).ToArray();
        }

        private CommandHandlerInvoker GetTheCommandHandler(ICommand command)
        {
            if (!commandInvokers.TryGetValue(command.GetType(), out CommandHandlerInvoker commandInvoker))
                throw new CommandHandlerNotFoundException(command.GetType());
            return commandInvoker;
        }

        public class CommandHandlerInvoker
        {
            readonly Type commandHandlerType;
            readonly Type commandType;

            public CommandHandlerInvoker(Type commandType, Type commandHandlerType)
            {
                this.commandType = commandType;
                this.commandHandlerType = commandHandlerType;
            }

            public Task SendAsync(IServiceProvider serviceProvider, ICommand command, IDictionary<string, object> headers, string correlationId)
            {
                var handlingContext = CreateTheCommandHandlingContext(command, headers, correlationId);
                return ExecuteTheCommandHandlerAsync(serviceProvider, handlingContext);
            }

            public async Task<TResult> AskAsync<TResult>(IServiceProvider serviceProvider, ICommand command, IDictionary<string, object> headers, string correlationId)
            {
                var handlingContext = CreateTheCommandHandlingContext(command, headers, correlationId);
                await ExecuteTheCommandHandlerAsync(serviceProvider, handlingContext);
                return handlingContext.GetReturn<TResult>();
            }

            private async Task ExecuteTheCommandHandlerAsync(IServiceProvider serviceProvider, ICommandHandlingContext<ICommand> handlingContext)
            {
                var handleMethod = GetTheHandleMethod();
                var commandHandler = serviceProvider.GetService(commandHandlerType);

                try
                {
                    await (Task)handleMethod.Invoke(commandHandler, new object[] { handlingContext });
                    return;
                }
                catch (TargetInvocationException ex)
                {
                    throw new CommandHandlerInvocationException(
                        string.Format("Command handler '{0}' for '{1}' failed. Inspect inner exception.", commandHandler.GetType().Name, handlingContext.Command.GetType().Name),
                        ex.InnerException)
                        {
                            HandlingContext = handlingContext
                        };
                }
            }

            private ICommandHandlingContext<ICommand> CreateTheCommandHandlingContext(ICommand command, IDictionary<string, object> headers, string correlationId)
            {
                var handlingContextType = typeof(CommandHandlingContext<>).MakeGenericType(commandType);
                return (ICommandHandlingContext<ICommand>)Activator.CreateInstance(handlingContextType, command, headers, correlationId);
            }

            private MethodInfo GetTheHandleMethod()
            {
                return typeof(IHandleCommands<>).MakeGenericType(commandType).GetTypeInfo().GetDeclaredMethod("Handle");
            }
        }

        private class CommandHandlingContext<TCommand> : ICommandHandlingContext<TCommand>
            where TCommand : ICommand
        {
            public CommandHandlingContext(TCommand command, IDictionary<string, object> headers, string correlationId)
            {
                Command = command;
                Headers = headers;
                CorrelationId = correlationId;
            }

            public IDictionary<string, object> Headers { get; private set; }

            public TCommand Command { get; private set; }

            public string CorrelationId { get; private set; }

            private object _returnValue;

            public T GetReturn<T>()
            {
                return (T)_returnValue;
            }

            public void SetReturn<T>(T value)
            {
                _returnValue = value;
            }
        }
    }
}
