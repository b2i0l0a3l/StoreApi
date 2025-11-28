using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using StoreSystem.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace StoreSystem.Infrastructure.Email
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private ILogger<SmtpEmailSender> _logger;
        public SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            // Read from environment variables first, then fall back to configuration
            var host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? _config["Smtp:Host"];
            var port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? _config["Smtp:Port"] ?? "25");
            var user = Environment.GetEnvironmentVariable("SMTP_USER") ?? _config["Smtp:User"];
            var pass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? _config["Smtp:Pass"];
            var from = Environment.GetEnvironmentVariable("SMTP_FROM") ?? _config["Smtp:From"] ?? user;

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                _logger.LogError("SMTP configuration is missing. Email not sent.");
                return;
            }
            
            using var client = new SmtpClient(host, port)
            {
                EnableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL") ?? _config["Smtp:EnableSsl"], out var ssl) && ssl,
                Credentials = new NetworkCredential(user, pass)
            };

            var mail = new MailMessage(from!, to, subject, htmlBody) { IsBodyHtml = true };
            await client.SendMailAsync(mail);
        }
    }
}
