using NBasis.Querying;

namespace Microsoft.AspNetCore.Mvc;

public static class QueryResultExtensions
{
    public static async Task<IActionResult> DispatchQueryAsync<TQuery, TResult>(
        this Controller controller,
        TQuery query,
        Func<TResult, Task<IActionResult>> success = null,
        Func<Task<IActionResult>> noresult = null,
        Func<Exception, Task<IActionResult>> error = null)
        where TQuery : IQuery<TResult>
    {
        var dispatcher = controller.HttpContext.RequestServices.GetRequiredService<IQueryDispatcher>();

        try
        {
            // execute query
            TResult result = await dispatcher.DispatchAsync(query, controller.HttpContext.User.GetUserHeader());

            // no result
            if (result == null)
            {
                return (noresult != null) ? await noresult() : controller.NotFound();
            }
            else // has result
            {
                return (success != null) ? await success(result) : controller.Ok(result);
            }
        }
        catch (Exception ex)
        {
            if (error != null)
                return await error(ex);

            if (ex is QueryUnauthorizedException)
                return controller.Forbid();

            // else throw the exception
            throw;
        }
    }
}