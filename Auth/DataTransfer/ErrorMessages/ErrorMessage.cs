
public class ErrorMessage {
    public string Type { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }

     public ErrorMessage(string type, string message, int statusCode = 400)
    {
        Type = type;
        Message = message;
        StatusCode = statusCode;
    }
}




