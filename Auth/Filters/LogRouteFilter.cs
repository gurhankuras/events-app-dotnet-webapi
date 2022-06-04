using Microsoft.AspNetCore.Mvc.Filters;

public class LogRouteFilter : IAsyncActionFilter
{
    private readonly ILogger<LogRouteFilter> _logger;

    public LogRouteFilter(ILogger<LogRouteFilter> logger)
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