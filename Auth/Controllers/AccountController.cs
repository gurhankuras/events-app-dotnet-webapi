
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Auth.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

/*
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IMapper _mapper;

    public AccountController(IUserRepository userRepo, IEmailSender emailSender, IMapper mapper) 
    {
        _userRepo = userRepo;
        _mapper = mapper;
    }



    [HttpGet("verify")]
    public async Task<object> Verify([FromQuery] string token)
    {  
        var validationResult = await ValidateEmailVerificationToken(token);
        var userId = validationResult.Claims.FirstOrDefault(claim => claim.Key == "id").Value;
        if (validationResult.IsValid && userId is not null) 
        {   
            var verified = await _userRepo.Verify(userId.ToString(), token);
            if (verified) {
                return new {Message="Success"};
            }
            return new {Message="Fail"};
        }

        Console.WriteLine(validationResult.Exception.Message);
        return new {Message="Fail"};
    }

    private async Task<TokenValidationResult> ValidateEmailVerificationToken(string token) 
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
        return validationResult;
    }
}
    
    */