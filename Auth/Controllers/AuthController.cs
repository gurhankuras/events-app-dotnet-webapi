
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Dto;
using Auth.Jwt;
using Auth.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[ApiController]
[Route("[controller]")]

public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthenticationService _authenticationService;
    private readonly IMapper _mapper;

    public AuthController(IUserRepository userRepo, IAuthenticationService authenticationService, IMapper mapper) 
    {
        _userRepo = userRepo;
        _authenticationService = authenticationService;
        _mapper = mapper;
    }



    [HttpPost("signin")]
    public async Task<ActionResult<SignInResponse>> SignIn([FromBody] SignInRequest body)
    {
        return new SignInResponse {
            Id = Guid.NewGuid().ToString(),
            Email = body.Email
        };
    }

     private string generateJwtToken(string secret, string id)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", id) }),
            //Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    //[TypeFilter(typeof(CustomAuthorizationFilter))]
    [Authorize]
    [HttpGet("secure")]
    public Dictionary<String, String> Secure()
    {
        return new Dictionary<String, String> 
        {
            {"message", "OK"}
        };
    }

     //[TypeFilter(typeof(CustomAuthorizationFilter))]
    [Authorize(Roles = "premium")]
    [HttpGet("premium")]
    public Dictionary<String, String> Premium()
    {
        return new Dictionary<String, String> 
        {
            {"message", "OK"}
        };
    }


    [HttpGet("deneme")]
    public async Task<ActionResult<FindByIdUserResponse>> Deneme([FromQuery] string id)
    {
        var user = await _userRepo.FindById(id);
        if (user is null) {
            return NotFound(new { message = $"User with {id} not found!"});
        }
        return new FindByIdUserResponse 
        {
            Id = user.Id.ToString(),
            Email = user.Email
        };
    }

    [HttpGet("jwt")]
    public dynamic JwtDeneme()
    {
        var bearerExists = HttpContext.Request.Headers.TryGetValue("Authorization", out var bearerText);
        if (!bearerExists) {
            return new {Deneme = 1};
        } 
        var token = SanitazeBearerOrDefault(bearerText);
        if (string.IsNullOrEmpty(token)) {
            return new {Deneme = 1};
        }
        var jwtService = new JwtService();
        var payload = jwtService.GetPayload(token);
        return payload;
    }

    private string? SanitazeBearerOrDefault(string bearerText) 
    {
        var pieces = bearerText.ToString().Split(" ");
         if (pieces.Length == 2) {
            var token = pieces.Last();
            return token;
        }
        return null;
    }
}
