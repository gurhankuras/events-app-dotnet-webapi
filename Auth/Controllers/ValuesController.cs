using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Web;
using Auth.AsyncServices;
using Auth.Dto;
using Auth.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace mongoidentity.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationService _tokenGenerator;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _busClient;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<ApplicationUser> userManager, 
                            RoleManager<ApplicationRole> roleManager,
                            SignInManager<ApplicationUser> signInManager,
                            IAuthenticationService tokenGenerator,
                            IMapper mapper,
                            IMessageBusClient busClient,
                            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
            _mapper = mapper;
            _busClient = busClient;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("demo")]
        public async Task<ActionResult> deeeneme() {
            
            Console.WriteLine(User.FindFirst("id").Value);
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<dynamic>> GetCurrentUserProfile() 
        {
            var id = User.FindFirst("id")?.Value;

            if (id is null) 
            {
                _logger.LogCritical("Token does not include id. Authentication mechanism doesn't work");
                return BadRequest("Token does not include id");
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user is null) 
            {
                return BadRequest("User not found");
            }

            return Ok(new {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Linkedin = user.LinkedinInfo 
            });
        }

        [HttpPost]
        public async Task<ActionResult<dynamic>> Create(User user)
        {
            Console.WriteLine(nameof(Create));
            if (!ModelState.IsValid)
            {
                Console.WriteLine(ModelState);
                return BadRequest();
            }
            Console.WriteLine(HttpContext.Request.Headers["token"]);
            Console.WriteLine(user);

            var appUser = new ApplicationUser
            {
                UserName = user.Name,
                Email = user.EmailAddress
            };

            var result = await _userManager.CreateAsync(appUser, user.Password);
            if (result.Succeeded)
            {
                //_busClient.PublishUser(_mapper.Map<UserPublishedDto>(appUser));
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                //var r = await _userManager.ConfirmEmailAsync(appUser, code);
                
                var userId = appUser.Id.ToString();

                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var encodedUserId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));
        
                var urlBuilder = new UriBuilder();
                urlBuilder.Host = "localhost";
                urlBuilder.Path = "account/email/verify";
                urlBuilder.Scheme = "http";
                urlBuilder.Port = 5000;
                urlBuilder.Query = $"userId={encodedUserId}&token={encodedToken}";


                Console.WriteLine(urlBuilder.Uri);
                return StatusCode(201, _mapper.Map<SignInUserResponse>(appUser));
            }

            return StatusCode(400, result.Errors.FirstOrDefault());
        }

        [HttpGet("email/verify")]
        public async Task<ActionResult<dynamic>> ConfirmEmail([FromQuery] string token, [FromQuery] string userId)
        {
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var decodedUserId = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userId));

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var verifier = new IdentityEmailVerifier(_userManager);
            var isVerified = await verifier.Verify(decodedUserId, decodedToken);
            return isVerified;
        }

        [HttpPost("roles")]
        public async Task<ActionResult<dynamic>> CreateRole(UserRole role) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }
            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = role.RoleName });
            if (!result.Succeeded) 
            {
                return StatusCode(400, result.Errors.FirstOrDefault());
            }

            return Ok(new { Message = "Success" });
        }

        [HttpPost("deneme")]
        public async Task<ActionResult<dynamic>> UpdateToPremium(UpdateToPremiumRequest body) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(body.UserId);
            if (user == null) {
                return StatusCode(400, new { Message = "User not found"} );
            }

            var result = await _userManager.AddToRoleAsync(user, "premium");
            //var result = await _roleManager.CreateAsync(new ApplicationRole { Name = role.RoleName });
            if (!result.Succeeded) 
            {
                return StatusCode(400, result.Errors.FirstOrDefault());
            }

            return Ok(new { Message = "Success" });
        }

        [HttpPost("signin")]
        public async Task<ActionResult<dynamic>> SignIn([FromBody] SignInRequest body)
        {

            var user = await _userManager.FindByEmailAsync(body.Email);
            if (user == null) {
                return StatusCode(404, new { Message = "User not found"} );
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, body.Password, lockoutOnFailure: false);

            if (result.Succeeded) 
            {
                
                var token = await _tokenGenerator.GenerateAccessToken(user, _userManager);
                
                HttpContext.Response.AddAuthHeader(token);
                return _mapper.Map<SignInUserResponse>(user);
            }

            return StatusCode(400, new { Message = "Password is not correct"} );
        }
    }
}

public class UpdateToPremiumRequest {
    [Required]
    public string UserId { get; set; }
}
