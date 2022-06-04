using Microsoft.AspNetCore.Identity;
using mongoidentity;

namespace Auth.Jwt;

public interface IAuthenticationService {
    Task<string> GenerateAccessToken(ApplicationUser user, UserManager<ApplicationUser> userManager);
}
