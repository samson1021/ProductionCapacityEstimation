using System;
using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using mechanical.Models.Dto.MailDto;

namespace mechanical.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly string _systemEmail;
        private readonly string _systemPassword;

        public MailService(ILogger<MailService> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            // Load SMTP settings from configuration or environment variables
            _smtpHost = configuration["Smtp:Host"] ?? "mail.cbe.com.et";
            _smtpPort = int.TryParse(configuration["Smtp:Port"], out var port) ? port : 587;
            _enableSsl = bool.TryParse(configuration["Smtp:EnableSsl"], out var ssl) ? ssl : true;
            _systemEmail = configuration["Smtp:Email"] ?? throw new ArgumentNullException("Smtp:Email");
            _systemPassword = configuration["Smtp:Password"] ?? throw new ArgumentNullException("Smtp:Password");

            // // Load SMTP settings from environment variables or use defaults
            // _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "mail.cbe.com.et";
            // _smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 587;
            // _enableSsl = bool.TryParse(Environment.GetEnvironmentVariable("SMTP_ENABLESSL"), out var ssl) ? ssl : true;
            // _systemEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? throw new ArgumentNullException("SMTP_EMAIL");
            // _systemPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new ArgumentNullException("SMTP_PASSWORD");

            if (string.IsNullOrWhiteSpace(_systemEmail) || string.IsNullOrWhiteSpace(_systemPassword))
            {
                throw new ArgumentNullException("SMTP Email or Password is not configured properly.");
            }
        }

        public async Task<bool> SendEmail(string recipientEmail, string subject, string body)
        {
            if (!IsValidEmail(_systemEmail) || !IsValidEmail(recipientEmail))
                return false;
            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
                return false;

            try
            {
                var message = BuildMailMessage(recipientEmail, subject, body);
                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    EnableSsl = _enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_systemEmail, _systemPassword)
                };
                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error sending email from {Sender} to {Recipient}", _systemEmail, recipientEmail);
                return false;
            }
        }

        private MailMessage BuildMailMessage(string recipientEmail, string subject, string body)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_systemEmail),
                Subject = HtmlEncoder.Default.Encode(subject),
                IsBodyHtml = true
            };
            message.To.Add(recipientEmail);
            message.Body = BuildHtmlBody(body);
            return message;
        }

        private string BuildHtmlBody(string body)
        {
            // HTML encode the body to prevent XSS, but allow basic formatting
            var safeBody = HtmlEncoder.Default.Encode(body).Replace("\n", "<br/>");
            return $"<h3 style=\"color:purple\">Case Valuation Schedule!:</h3><ul><li style=\"color:black\">{safeBody}</li></ul>";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
