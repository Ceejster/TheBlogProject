namespace TheBlogProject.ViewModels
{
    public class MailSettings
    {
        public required string Email { get; set; }
        public string? DisplayName { get; set; }
        public required string SendGridApiKey { get; set; }

    }
}
