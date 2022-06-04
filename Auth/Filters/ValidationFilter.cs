
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errorsInState = context.ModelState
                                .Where(x => x.Value?.Errors.Count > 0)
                                .ToDictionary(
                                    kvp => kvp.Key, 
                                    kvp => kvp.Value?.Errors.Select(x => x.ErrorMessage)
                                )
                                .ToArray();
            var errorResponse = new ValidationErrorMessage();
            foreach (var error in errorsInState)
            {
                if (error.Value is not null )
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new ValidationErrorMessage.Error();
                        errorModel.FieldName = error.Key;
                        errorModel.Description = subError;
                        errorResponse.Errors.Add(errorModel);
                    }
                }
               
            }
            context.Result = new BadRequestObjectResult(errorResponse);
            return;
        }
        // before
        await next();
        // after
    }
}