namespace NBasis.Handling
{
    public interface IHandler<TInput, TOutput>
    {
        Task<TOutput> HandleAsync(IHandlingContext<TInput, TOutput> handlingContext);
    }
}
