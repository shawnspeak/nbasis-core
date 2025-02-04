using System.Security.Claims;

namespace NBasis.Commanding;

public static class CommandExtensions
{
    public static ClaimsPrincipal GetClaimsPrincipal<TCommand, TResult>(this CommandHandler<TCommand, TResult> handler)
            where TCommand : ICommand<TResult>
    {
        if (handler.Headers.TryGetValue("User", out object value))
        {
            return value as ClaimsPrincipal;
        }
        return null;
    }
}