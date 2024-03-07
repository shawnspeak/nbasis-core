using NBasis.Handling;

namespace NBasis.Commanding
{
    public class CommandHandlerInvocationException<TCommand, TOutput> : Exception
    {
        public IHandlingContext<TCommand, TOutput> HandlingContext { get; set; }

        public CommandHandlerInvocationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
