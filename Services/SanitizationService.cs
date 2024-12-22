using Ganss.Xss;

namespace TheBlogProject.Services
{
    public class SanitizationService : ISanitizeService
    {
        private readonly HtmlSanitizer _sanitizer;

        public SanitizationService()
        {
            _sanitizer = new HtmlSanitizer();

            _sanitizer.AllowedTags.Add("p");
            _sanitizer.AllowedTags.Add("strong");
            _sanitizer.AllowedTags.Add("em");
            _sanitizer.AllowedTags.Add("ul");
            _sanitizer.AllowedTags.Add("li");
            _sanitizer.AllowedTags.Add("a");
            _sanitizer.AllowedAttributes.Add("href");
            _sanitizer.AllowedAttributes.Add("title");
        }

        public string Sanitize(string htmlContent)
        {
            return _sanitizer.Sanitize(htmlContent);
        }
    }
}
