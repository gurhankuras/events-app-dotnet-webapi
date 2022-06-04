using Auth.Config;
using Auth.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using mongoidentity;

namespace Auth.Jwt;

static class AddJwtServiceExtension {
    public static IServiceCollection AddJwtAuthService(this IServiceCollection services, IConfiguration configuration) 
    {   
        var jwtConfig = configuration.GetSection(nameof(JwtConfig)).Get<JwtConfig>() 
                        ?? throw new ArgumentNullException(nameof(JwtConfig));
        return services.AddSingleton<IAuthenticationService, JwtAuthenticationService>(
            (_) => new JwtAuthenticationService(jwtConfig.Key)
        );
    }
}