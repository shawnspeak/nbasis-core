namespace NBasis.Handling
{
    public interface IHandlingContext<TInput, TOutput>
    {
        TInput Input { get; }

        IDictionary<string, object> Headers { get; }

        TOutput Output { get; }
    }
}
