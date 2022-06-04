namespace Auth.Config;
public class AWSEmailSettings
{
    public string EmailSenderEmail { get; set; }
    public  string SmtpHost { get; set; }
    public  string SmtpUser { get; set; }
    public string SmtpPassword { get; set; }
    public int SmtpPort { get; set; }
    public int SmtpTTL { get; set; }
}