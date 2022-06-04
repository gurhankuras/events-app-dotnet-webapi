public class ValidationErrorMessage: ErrorMessage 
{
    public ValidationErrorMessage(
        string type = "ValidationError", 
        string message =  "One or more validation errors occurred.", 
        int statusCode = 400
    ) : base(type, message,statusCode)
    {

    }

    public List<Error> Errors { get; set; } = new List<Error>();
    
    public class Error 
    {
        public string? FieldName { get; set; }
        public string Description { get; set; }
    }
}