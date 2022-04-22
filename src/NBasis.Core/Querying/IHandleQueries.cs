namespace NBasis.Querying
{
    public interface IHandleQueries
    {
    }

    public interface IHandleQueries<TQuery, TResult> : IHandleQueries where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(IQueryHandlingContext<TQuery, TResult> handlingContext);
    }
}
