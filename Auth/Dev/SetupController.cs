using Auth.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mongoidentity;

[Route("[controller]")]
    [ApiController]
    public class SetupController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthenticationService _tokenGenerator;
        public SetupController(UserManager<ApplicationUser> userManager, 
                            RoleManager<ApplicationRole> roleManager,
                            SignInManager<ApplicationUser> signInManager,
                            IAuthenticationService tokenGenerator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("premium")]
        public async Task<Boolean> CreatePremiumRole() 
        {
            var result = await _roleManager.CreateAsync(new ApplicationRole { Name = "premium" });
            if (!result.Succeeded) {
                return false;
            }
            return true;
        }
    }