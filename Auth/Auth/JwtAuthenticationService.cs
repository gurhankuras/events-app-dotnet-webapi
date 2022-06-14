using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using mongoidentity;

namespace Auth.Jwt;

public class JwtAuthenticationService: IAuthenticationService {
    private readonly string _key;


    public JwtAuthenticationService(string key)
    {
        _key = key;
    }

    public async Task<string> GenerateAccessToken(ApplicationUser user, UserManager<ApplicationUser> userManager) 
    {
        var handler = new JwtSecurityTokenHandler();
        var encodedKey = Encoding.UTF8.GetBytes(_key);
        var securityKey = new SymmetricSecurityKey(encodedKey); 
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim> {
            new Claim("id", user.Id.ToString()),
            new Claim("email", user.Email), 
            new Claim("image", FileStorageUtils.getProfileImageURL(user.Id.ToString()))       
        };

        var roles = await userManager.GetRolesAsync(user);
        AddRolesToClaims(claims, roles);
        //if (role != null) {
            //claims.Add(new Claim(ClaimTypes.Role, "premium"));
            //claims.Add(new Claim(ClaimTypes.Role, "mod"));

        //}
        var claimsIdentity = new ClaimsIdentity(claims);
    
        var descriptor = new SecurityTokenDescriptor {
            Subject = claimsIdentity,
            Expires = DateTime.UtcNow.AddMinutes(30),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = credentials
        };
        handler.SetDefaultTimesOnTokenCreation = false;
        var token = handler.CreateToken(descriptor);
       
        return handler.WriteToken(token);
    }

     private void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }
}