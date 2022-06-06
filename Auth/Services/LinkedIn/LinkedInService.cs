using System.Text.Json;
using System.Web;
using Auth.Dto;
using Microsoft.AspNetCore.Identity;
using mongoidentity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Auth.Linkedin;

public interface ILinkedInService 
{
    Task<bool> VerifyUser(string userId, LinkedInVerificationRequest request);
}




public class LinkedInAccessToken 
{
    public string Token { get; set; }
    public int ExpiresIn { get; set; }

    public string Expires() {
        var date = DateTime.UtcNow;
        date = date.AddSeconds(ExpiresIn);
        return date.ToUniversalTime().ToString("u").Replace(" ", "T");
    }
}

public class LinkedInAccessTokenResponse
{
    [JsonRequired]
    public string AccessToken { get; set; }
    [JsonRequired]
    public int ExpiresIn { get; set; }
}

public class TokenNotFoundException: Exception {
    public TokenNotFoundException(string message): base(message)
    {
        
    }
}

public class LinkedinService : ILinkedInService
{
    private readonly ILinkedinClient _client;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<LinkedinService> _logger;

    public LinkedinService(ILinkedinClient client, UserManager<ApplicationUser> userManager, ILogger<LinkedinService> logger)
    {
        _client = client;
        _userManager = userManager;
        _logger = logger;
    }

    
    public async Task<bool> VerifyUser(string userId, LinkedInVerificationRequest request)
    {
        var token = await _client.GetAccessToken(request);
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) {
            _logger.LogTrace($"User with {userId} not found");
            throw new UserNotFound(userId);
        }
        user.LinkedinInfo = new LinkedInInfo {
            AccessToken = token.Token,
            Expires = token.Expires()
        };
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded) 
        {
            _logger.LogTrace(result?.Errors.FirstOrDefault()?.Description ?? "An error occured!");
            return false;
        }
        return true;
    }
}

public class UserNotFound : Exception
{
    public string UserId { get; }
    public UserNotFound(string userId): base($"User with {userId} not found")
    {
        UserId = userId;
    }
}