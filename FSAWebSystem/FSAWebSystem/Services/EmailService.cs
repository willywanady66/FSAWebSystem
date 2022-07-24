using FSAWebSystem.Models.Context;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FSAWebSystem.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailContext _emailContext;

        public EmailService(IOptions<EmailContext> emailContext)
        {
            _emailContext = emailContext.Value;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailContext.Name, _emailContext.Email));
            emailMessage.To.Add(new MailboxAddress(email, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate(_emailContext.Email, _emailContext.Password);
                client.Send(emailMessage);
                client.Disconnect(true);
            }

            return Task.FromResult(true);
        }
    }
}
