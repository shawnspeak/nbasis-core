using System.Reflection;
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
        /// Called when the commander has succeeded
        /// </summary>
        public Func<OptionContext, Task> OnSuccess = null;

        /// <summary>
        /// Catch all exceptions in the commander
        /// </summary>
        /// <remarks>If handled, then the delegate can return true to stop the commander from throwing the exception</remarks>
        public Func<Exception, Task<bool>> OnException = null;
    }

    public static class CommanderExtensions
    {
        public static Task<TResult> AskAsync<TResult>(this ICommander commander, ICommand<TResult> command, IDictionary<string, object> headers = null)
        {
             // get type of command
            var commandType = command.GetType();
            var resultType = typeof(TResult);

            // build envelope
            var envelopeType = typeof(Envelope<>).MakeGenericType(commandType);
            var envelope = Activator.CreateInstance(envelopeType, command, headers);

            var methodInfo = typeof(ICommander).GetTypeInfo().GetDeclaredMethod("AskAsync");
            
            return (Task<TResult>)methodInfo.MakeGenericMethod(commandType, resultType).Invoke(commander, new[] { envelope });
        }

        public static IServiceCollection AddLocalCommanding(this IServiceCollection serviceCollection, CommandOptions options = null)
        {
            return serviceCollection
                    .AddSingleton<ICommanderFactory, LocalCommanderFactory>()
                    .AddScoped<ICommander>((sp) =>
                    {
                        return sp.GetService<ICommanderFactory>().GetCommander(sp, options);
                    });
        }
    }
}
