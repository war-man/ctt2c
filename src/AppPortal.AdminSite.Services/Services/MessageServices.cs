﻿using System.Threading.Tasks;
using AppPortal.AdminSite.Services.Interfaces;

using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace AppPortal.AdminSite.Services
{
    public class AuthMessageSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message, string fullName = null, string userName = null, string passWord = null)
        {
            // Plug in your email service here to send an email.
            var messages = new MimeMessage();
            messages.From.Add(new MailboxAddress("Portal@Contact-no-reply", userName));
            messages.To.Add(new MailboxAddress($"Mr/Mrs {fullName}", email));
            messages.Subject = subject;

            messages.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<b>Hi {fullName}!</b> <br/>" +                
                $"{message}" +
                $"<br/><br/><br/><br/> -- no-reply!"
            };
            try
            {
                using (var client = new SmtpClient())
                {
                    // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    client.Connect("smtp.gmail.com", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate(userName, passWord);

                    client.SendAsync(messages).Wait();
                    client.Disconnect(true);
                }
            }
            catch
            {                
            }            
            return Task.FromResult(0);
        }
    }
}
