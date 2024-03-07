namespace NBasis.Querying
{
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatch a query and wait for a result
        /// </summary>
        Task<TResult> DispatchAsync<TQuery, TResult>(Envelope<TQuery> query) where TQuery : IQuery<TResult>;
    }
}
