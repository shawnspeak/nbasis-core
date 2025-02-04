using System.Security.Claims;
using NBasis.Commanding;
using NBasis.Querying;

namespace Microsoft.AspNetCore.Mvc;

public static class MvcExtensions
{
    public static IDictionary<string, object> GetUserHeader(this ClaimsPrincipal user)
    {
        IDictionary<string, object> headers = new Dictionary<string, object>();
        if (user != null)
        {
            headers = new Dictionary<string, object>
            {
                { "User", user },
            };
        }
        return headers;
    }

    public static Task<TResponse> DispatchQuery<TQuery, TResponse>(this Controller controller, TQuery query) where TQuery : IQuery<TResponse>
    {
        var dispatcher = controller.HttpContext.RequestServices.GetRequiredService<IQueryDispatcher>();
        return dispatcher.DispatchAsync<TResponse>(query, controller.HttpContext.User.GetUserHeader());
    }

    public static Task<TResponse> AskCommand<TCommand, TResponse>(this Controller controller, TCommand command) where TCommand : ICommand<TResponse>
    {
        var commander = controller.HttpContext.RequestServices.GetRequiredService<ICommander>();
        return commander.AskAsync<TResponse>(command, controller.HttpContext.User.GetUserHeader());
    }
}
