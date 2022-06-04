/*
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Services;
using Microsoft.IdentityModel.Tokens;

public interface IAccountEmailVerifier 
{
    Task Verify(string token);
}

public class AccountEmailVerifier : IAccountEmailVerifier
{
    private readonly IUserRepository userRepository;

    public AccountEmailVerifier(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }
    public async Task Verify(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var validationParams = new TokenValidationParameters() 
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("secretsecretsecre")),
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false
        };
        
        var validationResult = await handler.ValidateTokenAsync(token, validationParams);
        var userId = validationResult.Claims.FirstOrDefault(claim => claim.Key == "id").Value;
        if (validationResult.IsValid && userId is not null) 
        {   
           
        }

        Console.WriteLine(validationResult.Exception.Message);
        //return new {Message="Fail"};
    }
}
*/