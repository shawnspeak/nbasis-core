
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using NBasis.Commanding;

namespace Microsoft.AspNetCore.Mvc;

public static class CommandResultExtensions
{
    public static Func<Exception, Task<IActionResult>> ErrorView(this Controller controller)
    {
        return (ex) =>
        {
            return Task.FromResult(controller.Ok() as IActionResult);
        };
    }

    public class BadRequestBody
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Code { get; set; }
    }

    public static Func<Exception, Task<IActionResult>> BadRequestJson(this Controller controller)
    {
        return (ex) =>
        {
            var jsonBody = new BadRequestBody { Message = ex.Message };

            if (ex is CommandValidationException cex)
            {
                jsonBody.Code = cex.Code;
            }

            return Task.FromResult(controller.BadRequest(jsonBody) as IActionResult);
        };
    }

    public static async Task<IActionResult> AskCommandAsync<TCommand, TResult>(
        this Controller controller,
        TCommand command,
        Func<TResult, Task<IActionResult>> success = null,
        Func<Exception, Task<IActionResult>> error = null)
        where TCommand : ICommand<TResult>
    {
        var commander = controller.HttpContext.RequestServices.GetRequiredService<ICommander>();

        controller.ModelState.Clear();

        TResult result = default;
        Exception ex = null;
        if (controller.TryValidateModel(command))
        {
            try
            {
                result = await commander.AskAsync<TResult>(command, controller.HttpContext.User.GetUserHeader());
            }
            catch (CommandHandlerInvocationException<TCommand, TResult> chie)
            {
                ex = chie.InnerException;
            }
            catch (Exception cex)
            {
                ex = cex;
            }

            if (ex != null)
            {
                ILoggerFactory logFactory = controller.HttpContext.RequestServices.GetService<ILoggerFactory>();
                if (logFactory != null)
                {
                    var log = logFactory.CreateLogger(controller.GetType());
                    try
                    {

                        using (log.BeginScope(new Dictionary<string, object>
                        {
                            ["CommandJson"] = JsonSerializer.Serialize(command)
                        }))
                        {
                            log.LogError(ex, "Exception asking command: {0}", typeof(TCommand).Name);
                        }
                    }
                    catch (Exception)
                    {
                        log.LogError(ex, "Exception asking command: {0}.", typeof(TCommand).Name);
                    }
                }

                controller.ModelState.AddModelError("", ex.Message);

                // deal with some standard exceptions
                if (ex is CommandUnauthorizedException)
                    return new StatusCodeResult((int)HttpStatusCode.Forbidden);
                if (ex is CommandRootNotFoundException)
                    return new NotFoundResult();
                if (ex is CommandValidationException)
                {
                    if (error == null)
                    {
                        return await controller.BadRequestJson().Invoke(ex);
                    }
                    else
                    {
                        return await error(ex);
                    }
                }

                if (error == null)
                {
                    return await controller.ErrorView().Invoke(ex);
                }
                else
                {
                    return await error(ex);
                }
            }
        }
        else
        {
            // validate the command
            var validationContext = new ValidationContext(command, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(command, validationContext, validationResults, true);

            if (error == null)
            {
                return await controller.BadRequestJson().Invoke(ex);
            }
            else
            {
                return await error(null);
            }
        }

        if (success != null)
            return await success(result);

        // return result as json
        return new JsonResult(result);
    }
}