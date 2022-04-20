using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace NBasis.Querying
{
    public static class QueryExtensions
    {
        public static Task<TResult> DispatchAsync<TResult>(this IQueryDispatcher proc, IQuery<TResult> query, IDictionary<string, object> headers = null)
        {
            // get type of query
            var queryType = query.GetType();
            var resultType = typeof(TResult);

            // build envelope
            var envelopeType = typeof(Envelope<>).MakeGenericType(queryType);
            var envelope = Activator.CreateInstance(envelopeType, query, headers);

            var methodInfo = typeof(IQueryDispatcher).GetTypeInfo().GetDeclaredMethod("DispatchAsync");
            
            return (Task<TResult>)methodInfo.MakeGenericMethod(queryType, resultType).Invoke(proc, new[] { envelope });
        }

        public static IServiceCollection AddLocalQueryDispatcher(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                    .AddSingleton<IQueryDispatcherFactory, LocalQueryDispatcherFactory>()
                    .AddScoped<IQueryDispatcher>((sp) =>
                    {
                        return sp.GetService<IQueryDispatcherFactory>().GetQueryDispatcher(sp);
                    });
        }
    }
}
