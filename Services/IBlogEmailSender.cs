namespace TheBlogProject.Services
{
    public interface IBlogEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string plainTextContent, string htmlContent);
    }
}
