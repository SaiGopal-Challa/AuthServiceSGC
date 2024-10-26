using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AuthServiceSGC.Infrastructure.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<EmailResponseDTO> SendOtpEmailAsync(string toEmail, string otp)
        {
            EmailResponseDTO emailResponseDTO = new EmailResponseDTO();

            var smtpHost = _configuration["EmailSettings:SMTPHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SMTPPort"]);
            var smtpUsername = _configuration["EmailSettings:SMTPUsername"];
            var smtpPassword = _configuration["EmailSettings:SMTPPassword"];
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = "Your OTP Code",
                Body = $"Your OTP code is: {otp}",
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toEmail));

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true
            };

            try
            {
                await client.SendMailAsync(message);
                emailResponseDTO.Status = "OTP via Email sent successfully.";
                Console.WriteLine("OTP email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                throw;
            }
            return emailResponseDTO;
        }
    }

    public class EmailResponseDTO
    {
        public string? Status { get; set; }
        
    }
}
