
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Auth.Config;

public class AWSEmailSender : IEmailSender
{
    private readonly AWSEmailSettings _settings;
    private readonly IAmazonSimpleEmailService _client;
    public AWSEmailSender(AWSEmailSettings settings, IAmazonSimpleEmailService client)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

     public Task Send(string to, string subject, string html)
    {
        var request = new SendEmailRequest();
      
        request.Destination = new Destination(new List<string>() { to });
        request.Source = _settings.EmailSenderEmail;
        
        var body = new Body().WithHTML(html);
        var subjectContent = new Content(subject);
        request.Message = new Message(subjectContent, body);
        
        return _client.SendEmailAsync(request);
    }

    public Task Register(string email)
    {
        var request = new VerifyEmailIdentityRequest {
            EmailAddress = email
        };
        return _client.VerifyEmailIdentityAsync(request);
    }	
}

public static class AWSSesExtensions {
    public static Body WithHTML(this Body body, string html)
    {
        var bodyContent = new Content(html);
        body.Html = bodyContent;
        return body;
    }
}