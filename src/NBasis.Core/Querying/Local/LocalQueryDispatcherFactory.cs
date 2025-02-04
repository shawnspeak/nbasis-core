using NBasis.Handling;
using NBasis.Types;

namespace NBasis.Querying
{
    public class LocalQueryDispatcherFactory : IQueryDispatcherFactory
    {
        readonly InvokerResolver<IHandleQueries> _resolver;

        public LocalQueryDispatcherFactory(ITypeFinder typeFinder)
        {
            _resolver = new InvokerResolver<IHandleQueries>(typeFinder);
        }

        public IQueryDispatcher GetQueryDispatcher(IServiceProvider serviceProvider)
        {
            return new QueryDispatcher(serviceProvider, _resolver);
        }

        private class QueryDispatcher : IQueryDispatcher
        {
            readonly IServiceProvider _serviceProvider;

            readonly InvokerResolver<IHandleQueries> _resolver;

            public QueryDispatcher(IServiceProvider serviceProvider, InvokerResolver<IHandleQueries> resolver)
            {
                _serviceProvider = serviceProvider;
                _resolver = resolver;
            }

            public async Task<TResult> DispatchAsync<TQuery, TResult>(Envelope<TQuery> query)
                where TQuery : IQuery<TResult>
            {
                // exception if not found
                var handler = _resolver.GetTheHandler(query.Body.GetType()) ?? throw new QueryHandlerNotFoundException(query.Body.GetType());
                var context = await handler.Invoke<TQuery, TResult>(_serviceProvider, query.Body, query.Headers);
                return context.Output;
            }
        }
    }
}
