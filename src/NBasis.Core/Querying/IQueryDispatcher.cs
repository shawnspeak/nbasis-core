using System.Threading.Tasks;

namespace NBasis.Querying
{
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Dispatch a query
        /// </summary>
        Task<TResult> DispatchAsync<TQuery, TResult>(Envelope<TQuery> query) where TQuery : IQuery<TResult>;
    }
}
