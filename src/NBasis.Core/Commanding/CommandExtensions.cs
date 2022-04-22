using Microsoft.Extensions.DependencyInjection;

namespace NBasis.Commanding
{
    public class OptionContext
    {
        public int CommandLevel { get; private set; }

        internal OptionContext(int commandLevel)
        {
            CommandLevel = commandLevel;
        }
    }

    public class CommandOptions
    {
        /// <summary>
        /// Called when the bus has succeeded
        /// </summary>
        public Func<OptionContext, Task> OnSuccess = null;

        /// <summary>
        /// Catch all exceptions in the command bus
        /// </summary>
        /// <remarks>If handled, then the delegate can return true to stop the bus from throwing the exception</remarks>
        public Func<Exception, Task<bool>> OnException = null;
    }

    public static class CommandExtensions
    {
        public static Task SendAsync(this ICommandBus bus, Envelope<ICommand> envelopedCommand)
        {
            return bus.SendAsync(envelopedCommand.Yield());
        }

        public static Task SendAsync(this ICommandBus bus, ICommand command, IDictionary<string, object> headers = null)
        {
            return bus.SendAsync(new Envelope<ICommand>(command, headers));
        }

        public static Task<TResult> AskAsync<TResult>(this ICommandBus bus, ICommand command, IDictionary<string, object> headers = null)
        {
            return bus.AskAsync<TResult>(new Envelope<ICommand>(command, headers));
        }

        public static IServiceCollection AddLocalCommanding(this IServiceCollection serviceCollection, CommandOptions options = null)
        {
            return serviceCollection
                    .AddSingleton<ICommandBusFactory, LocalCommandBusFactory>()
                    .AddScoped<ICommandBus>((sp) =>
                    {
                        return sp.GetService<ICommandBusFactory>().GetCommandBus(sp, options);
                    });
        }
    }
}
