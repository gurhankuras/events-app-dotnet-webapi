using Microsoft.AspNetCore.Http;

public interface IUserProvider
{
    public string? Id { get; }
}

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserProvider(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public string? Id { 
        get => _httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value; 
    }
}