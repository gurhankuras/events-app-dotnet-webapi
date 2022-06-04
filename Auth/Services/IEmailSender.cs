public interface IEmailSender {
    Task Send(string to, string subject, string html);
    Task Register(string email);
}