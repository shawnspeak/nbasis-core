using NBasis.Handling;

namespace NBasis.Querying
{
    public class QueryHandlerInvocationException<TQuery, TOutput> : Exception
    {   public IHandlingContext<TQuery, TOutput> HandlingContext { get; set; }

        public QueryHandlerInvocationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
