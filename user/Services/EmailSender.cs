using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using user.Data.Entities;

namespace user.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IOptions<MailSettings> mailSettingsOptions,IConfiguration config)
        {
            _config = config;
            _mailSettings = mailSettingsOptions.Value;
        }
        private readonly MailSettings _mailSettings;
  
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            var emailMessenger = new MimeMessage ();
            emailMessenger.From.Add(new MailboxAddress("Nguyen Mau Tam", "fakesmtp@gmail.com"));
            emailMessenger.To.Add(new MailboxAddress("", email));
            emailMessenger.Subject = subject;
            emailMessenger.Body = new TextPart("plain") { Text = body };

            using (var client = new SmtpClient())
            {
                client.Connect(_mailSettings.Server, _mailSettings.Port, SecureSocketOptions.StartTls);
                client.Authenticate(_mailSettings.UserName, _mailSettings.Password);
                client.Send(emailMessenger);
                client.Disconnect(true);
               
            }
            

        }

    }
}
