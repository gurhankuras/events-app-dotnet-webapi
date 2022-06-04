
using System.Security.Claims;
using System.Text;
using Auth.Config;
using Auth.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Auth.JwtExtensions;
public static class AddJwtAuthenticationExtension
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration) 
    {   
        var jwtConfig = configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>() 
                        ?? throw new ArgumentNullException(nameof(JwtConfig));
        services
        .AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
     
        })
        .AddJwtBearer(options => {
            options.TokenValidationParameters = new TokenValidationParameters 
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
            };
        });
    }
}