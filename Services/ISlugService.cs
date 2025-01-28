namespace TheBlogProject.Services
{
    public interface ISlugService
    {
        // Converts a title into a URL-friendly slug
        string UrlFriendly(string title);

        // Checks if a slug is unique for a specific entity type
        bool IsUnique(string slug, string entityType);

        // Generates a slug from a name, optionally adding a prefix
        string GenerateSlug(string name, string? prefix = null);

        // Validates the slug for both format and uniqueness
        (bool IsValid, string? ErrorMessage) ValidateSlug(string slug, string entityType);
    }
}
