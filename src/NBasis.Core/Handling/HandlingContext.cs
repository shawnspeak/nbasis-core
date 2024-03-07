namespace NBasis.Handling
{
    internal class HandlingContext<TInput, TOutput> : IHandlingContext<TInput, TOutput>
    {
        public HandlingContext(TInput input, IDictionary<string, object> headers)
        {
            Input = input;
            Headers = headers;
        }

        public TInput Input { get; }

        public IDictionary<string, object> Headers { get; }

        public TOutput Output { get; private set; }

        public void SetOutput(TOutput output)
        {
            Output = output;
        }        
    }
}
