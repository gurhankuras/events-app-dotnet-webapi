using Auth.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class UploadController : ControllerBase
{
    private readonly AWSFileUploader _uploader;

    public UploadController(AWSFileUploader uploader) 
    {
        _uploader = uploader;
    }



    [Authorize]
    [HttpGet]
    public async Task<ActionResult<GetPresignedResponse>> GeneratePresignedURL([FromQuery] GeneratePresignedURLQuery query)
    {  
        var id = User.FindFirst("id")?.Value;

        if (id == null) 
        {
            return Unauthorized();
        }
        var url = await _uploader.GetUploadURL(query.Type, id);
        return new GetPresignedResponse { Url = url }; 
    }
}


public class GeneratePresignedURLQuery {
    public string Type { get; set; }
}
