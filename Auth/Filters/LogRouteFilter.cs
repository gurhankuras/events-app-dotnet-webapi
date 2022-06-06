using Microsoft.AspNetCore.Mvc.Filters;

public class RouteLoggerFilter : IAsyncActionFilter
{
    private readonly ILogger<RouteLoggerFilter> _logger;

    public RouteLoggerFilter(ILogger<RouteLoggerFilter> logger)
    {
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var method = context.HttpContext.Request.Method.ToString();
        var path = context.HttpContext.Request.Path.ToString();
        var query = context.HttpContext.Request.QueryString.ToString();
        var log = $"{method}: {path}?{query}";
        _logger.LogTrace(log);
        await next();
    }
}