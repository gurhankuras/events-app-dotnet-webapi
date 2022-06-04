
using Amazon.S3;
using Amazon.S3.Model;

public class AWSFileUploader {
    private readonly IAmazonS3 _client;
    public AWSFileUploader(IAmazonS3 client) 
    {
        _client = client;
    }

    public Task<string> GetUploadURL(string key) {

        GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
        {
            BucketName = "gkevents-app",
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.Now.AddMinutes(5)
        };

        // Get path for request
        string path = _client.GetPreSignedURL(request);
        return Task.FromResult(path);
    }
    
}