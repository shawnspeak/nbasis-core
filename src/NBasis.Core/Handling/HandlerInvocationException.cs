namespace NBasis.Handling
{
    public class HandlerInvocationException<TInput, TOutput> : Exception
    {
        public IHandlingContext<TInput, TOutput> HandlingContext { get; set; }

        public HandlerInvocationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
