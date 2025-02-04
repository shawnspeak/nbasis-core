using System.Security.Claims;

namespace NBasis.Querying;

public static class QueryExtensions
{
    public static ClaimsPrincipal GetClaimsPrincipal<TQuery, TResponse>(this QueryHandler<TQuery, TResponse> handler)
             where TQuery : IQuery<TResponse>
    {
        if (handler.Headers.TryGetValue("User", out object value))
        {
            return value as ClaimsPrincipal;
        }
        return null;
    }
}