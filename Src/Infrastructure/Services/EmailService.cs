using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Application.Common.ResultsModel;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class EmailService : IEmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration, SmtpClient smtpClient)
        {
            _configuration = configuration;
            _smtpClient = smtpClient;
            _fromEmail = _configuration.GetConnectionString("Email");
        }

        public async Task<Result> SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _configuration.GetConnectionString("SmtpServer");
            var port = int.Parse(_configuration.GetConnectionString("Port"));
            var password = _configuration.GetConnectionString("Password");

            _smtpClient.Host = smtpServer;
            _smtpClient.Port = port;
            _smtpClient.Credentials = new NetworkCredential(_fromEmail, password);
            _smtpClient.EnableSsl = true;

            try
            {
                var mailMessage = new MailMessage(_fromEmail, toEmail, subject, body);
                await _smtpClient.SendMailAsync(mailMessage);
                return Result.Success($"Registration Email successfully sent to {toEmail}");
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to send email to {toEmail}", ex);
            }
        }
    }
}