using Microsoft.AspNetCore.Identity;
using mongoidentity;

public interface IEmailVerifier 
{
    Task<Boolean> Verify(string userId, string token);
}

public class IdentityEmailVerifier: IEmailVerifier
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityEmailVerifier(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> Verify(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) 
        {
            return false;
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        foreach (var item in result.Errors)
        {
            Console.WriteLine(item.Description);
        }

        if (!result.Succeeded) 
        {
            return false;
        }
        return true;
    }
}