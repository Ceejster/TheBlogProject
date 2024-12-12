using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using SendGrid;
using MailSettings = TheBlogProject.ViewModels.MailSettings;

namespace TheBlogProject.Services
{
    public class BlogEmailSender : IBlogEmailSender
    {
        private readonly MailSettings _mailSettings;

        public BlogEmailSender(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent)
        {
            var client = new SendGridClient(_mailSettings.SendGridApiKey);
            var from = new EmailAddress(_mailSettings.Email, _mailSettings.DisplayName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await client.SendEmailAsync(msg);
        }

    }
}
