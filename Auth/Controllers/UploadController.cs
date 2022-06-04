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



    [HttpGet]
    public async Task<GetPresignedResponse> GeneratePresignedURL([FromQuery] GeneratePresignedURLQuery query)
    {  
        var url = await _uploader.GetUploadURL(query.Key);
        return new GetPresignedResponse { Url = url }; 
    }
}


public class GeneratePresignedURLQuery {
    public string Key { get; set; }
}
