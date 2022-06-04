namespace Auth.Exceptions;

public class LinkedinHTTPException: Exception {
    public int StatusCode { get; }
    public LinkedinHTTPException(int statusCode, string? reason = null): base( reason ?? "Linkedin API returned error response")
    {
        StatusCode = statusCode;
    }
}