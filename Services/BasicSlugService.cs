using System.Text;
using System.Text.RegularExpressions;
using TheBlogProject.Data;

namespace TheBlogProject.Services
{
    public class BasicSlugService : ISlugService
    {
        private readonly ApplicationDbContext _context;

        public BasicSlugService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if the slug is unique
        public bool IsUnique(string slug, string entityType)
        {
            return entityType.ToLower() switch
            {
                "post" => !_context.Posts.Any(p => p.Slug == slug),
                "blog" => !_context.Blogs.Any(b => b.Slug == slug),
                "destination" => !_context.Destination.Any(d => d.Slug == slug),
                _ => throw new ArgumentException("Invalid entity type.")
            };
        }

        // Generate a URL-friendly slug
        public string UrlFriendly(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            // Convert to lowercase
            var slug = title.ToLowerInvariant();

            // Normalize Unicode characters
            slug = slug.Normalize(NormalizationForm.FormD);
            slug = Regex.Replace(slug, @"\p{IsCombiningDiacriticalMarks}+", "");

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

            // Ensure length is not excessive
            return slug.Length > 50 ? slug.Substring(0, 50) : slug;
        }

        // Generate context-aware slug
        public string GenerateSlug(string name, string? prefix = null)
        {
            var slug = UrlFriendly(name);

            // Add prefix if provided
            if (!string.IsNullOrEmpty(prefix))
            {
                slug = $"{prefix}-{slug}";
            }

            // Validate the slug before returning
            if (!ValidateSlug(slug))
            {
                throw new InvalidOperationException("Generated slug is invalid.");
            }

            return slug;
        }

        private bool ValidateSlug(string slug)
        {
            // Validates a slug format (does not check uniqueness)
            return !string.IsNullOrWhiteSpace(slug) &&
                   !Regex.IsMatch(slug, @"-{2,}") &&            // No consecutive hyphens
                   slug.Length > 1 &&                          // More than 1 character
                   Regex.IsMatch(slug, @"^[a-z0-9-]+$");       // Valid characters only
        }

        // Validate and check uniqueness
        public (bool IsValid, string? ErrorMessage) ValidateSlug(string slug, string entityType)
        {
            // Validate format
            if (!ValidateSlug(slug))
            {
                return (false, "The slug is invalid or empty.");
            }

            // Check for uniqueness
            if (!IsUnique(slug, entityType))
            {
                return (false, "The slug provided is already being used.");
            }

            return (true, null);
        }
    }
}
