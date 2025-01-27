using Core.Interfaces.Services;
using Core.Data.Mail;

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // Vérification des paramètres d'entrée
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentNullException(nameof(toEmail), "L'adresse e-mail de destination ne doit pas être vide.");

            // Vérification et récupération sécurisée des paramètres SMTP
            string fromEmail = _smtpSettings.FromEmail 
                            ?? throw new InvalidOperationException("L'adresse e-mail de l'expéditeur est introuvable dans la configuration.");
            string fromName = _smtpSettings.FromName ?? string.Empty;    
            try
            {
                var fromAddress = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
                var toAddress = new MailAddress(toEmail);

                using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    EnableSsl = false, // Désactivé car Mailpit n'utilise pas SSL
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_smtpSettings.UserName, _smtpSettings.Password)
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                await smtpClient.SendMailAsync(message);
                Console.WriteLine("E-mail envoyé avec succès !");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi de l'e-mail : {ex.Message}");
                throw;
            }
        }
    }
}