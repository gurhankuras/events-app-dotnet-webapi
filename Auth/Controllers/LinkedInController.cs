using Auth.Exceptions;
using Auth.Linkedin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mongoidentity;

[ApiController]
[Route("linkedin")]
public class LinkedInController : ControllerBase
{   
    private readonly ILinkedInService _linkedInService;
    private readonly ILogger<LinkedInController> _logger;
    private readonly IUserProvider _userProvider;

    public LinkedInController(ILinkedInService linkedInService, 
    ILogger<LinkedInController> logger,
    IUserProvider userProvider) 
    {
        _linkedInService = linkedInService;
        _logger = logger;
        _userProvider = userProvider;
    }

    [Authorize]
    [HttpPost("accessToken")]
    public async Task<ActionResult<dynamic>> VerifyAsLinkedInUser(LinkedInVerificationRequest body)
    {  
        //var id = User.FindFirst("id")?.Value;
        var id = _userProvider.Id;
        if (id is null) 
        {
            _logger.LogCritical("Probably authentication doesn't work.");
            return BadRequest(new { Error = $"Id not exists in token" });
        }
        try
        {
            var success = await _linkedInService.VerifyUser(id, body);
            if (success) {
                return Ok(new { Message = "Basarili" });
            }
            // cant update user. Database related issue
            _logger.LogError("Cant update user. Might be a database related issue.");
            return StatusCode(500);
        }
        catch (LinkedinHTTPException ex)
        {
            return StatusCode(ex.StatusCode);
        }
        catch (UserNotFound)
        {
            var message = new ErrorMessage(type: "UserNotFound", $"User not found with {id}");
            return StatusCode(400, message);
        }
    }
}