namespace Auth.Dto;

public class LinkedInVerificationRequest 
{
    public string GrantType { get; set; }
    public string Code { get; set; }
    public string ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string RedirectUri { get; set; }
}