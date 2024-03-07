using NBasis.Handling;

namespace NBasis.Querying
{
    public interface IHandleQueries
    {
    }

    public interface IHandleQueries<TQuery, TResult> : IHandler<TQuery, TResult>, IHandleQueries where TQuery : IQuery<TResult>
    {
    }
}
