// Services/EmailSender.cs
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration; // Para ler appsettings

namespace MSPremiumProject.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        // Configurações de email (exemplo, ajuste conforme necessário)
        // Você deve colocar isto no seu appsettings.json
        // "EmailSettings": {
        //   "SmtpServer": "smtp.example.com",
        //   "SmtpPort": 587,
        //   "EnableSsl": true,
        //   "SenderName": "MSPremiumProject",
        //   "SenderEmail": "noreply@mspremiumproject.com",
        //   "SmtpUser": "your_smtp_username",
        //   "SmtpPass": "your_smtp_password"
        // }
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly bool _enableSsl;
        private readonly string _senderName;
        private readonly string _senderEmail;
        private readonly string _smtpUser;
        private readonly string _smtpPass;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpServer = _configuration["EmailSettings:SmtpServer"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);
            _senderName = _configuration["EmailSettings:SenderName"];
            _senderEmail = _configuration["EmailSettings:SenderEmail"];
            _smtpUser = _configuration["EmailSettings:SmtpUser"];
            _smtpPass = _configuration["EmailSettings:SmtpPass"];
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Validação básica
            if (string.IsNullOrEmpty(_smtpServer) || string.IsNullOrEmpty(_senderEmail))
            {
                // Logar um aviso ou erro - configuração de email em falta
                // Para este exemplo, apenas não enviaremos.
                Console.WriteLine("Configurações de SMTP não encontradas. Email não enviado.");
                return Task.CompletedTask;
            }

            var client = new SmtpClient(_smtpServer)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = _enableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_senderEmail, _senderName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            try
            {
                return client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Logar o erro
                Console.WriteLine($"Erro ao enviar email: {ex.Message}");
                // Pode querer lançar a exceção ou tratar de outra forma
                return Task.FromException(ex);
            }
        }
    }
}