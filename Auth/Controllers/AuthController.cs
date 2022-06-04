
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Jwt;
using Auth.Models;
using Auth.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

[ApiController]
[Route("[controller]")]

public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;
    private readonly IAuthenticationService _authenticationService;
    private readonly IEmailSender _emailSender;
    private readonly IMapper _mapper;

    public AuthController(IUserRepository userRepo, IAuthenticationService authenticationService, IEmailSender emailSender, IMapper mapper) 
    {
        _userRepo = userRepo;
        _authenticationService = authenticationService;
        _emailSender = emailSender;
        _mapper = mapper;
    }



    [HttpPost("signin")]
    public async Task<ActionResult<SignInResponse>> SignIn([FromBody] SignInRequest body)
    {
       //await _emailSender.Send("gurhankuras@hotmail.com", "C# deneme2", "<h1>Hello</h1>");
        //await _emailSender.Register("xecitij884@3dmasti.com");
        return new SignInResponse {
            Id = Guid.NewGuid().ToString(),
            Email = body.Email
        };
    }
    
    /*
    [HttpPost("signup")]
    public async Task<ActionResult<SignUpResponse>> SignUp([FromBody] SignUpRequest body)
    {

        var userExists = await _userRepo.IsUserExists(body.Email);
        if (userExists) {
            return BadRequest(new ErrorMessage("User already exists with provided email"));
        }
        var user = _mapper.Map<User>(body);
        
        try
        {
            
            await _userRepo.Create(user);
            var verificationToken = generateJwtToken("secretsecretsecre", user.Id.ToString());
            await _userRepo.SetEmailVerificationToken(user.Id.ToString(), verificationToken);
            var token = _authenticationService.GenerateAccessToken(user.Id.ToString(), email: user.Email, role: "standart");
            var verificationLink = generateEmailVerificationLink("https://google.com", verificationToken);
            Console.WriteLine(verificationToken);
            HttpContext.Response.AddAuthHeader(token);
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new ErrorMessage($"Bir hata oldu: {ex.Message}"));
        }
        
        return _mapper.Map<SignUpResponse>(user);
    }
    */

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

    private string generateEmailVerificationLink(string baseUrl, string token) 
    {
        return $"{baseUrl}?token={token}";
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
