using System.Diagnostics.Metrics;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;
using System;
using TheBlogProject.Data;
using TheBlogProject.Models;

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
        public bool IsUnique(string slug)
        {
            return !_context.Posts.Any(p => p.Slug == slug);
        }

        // Generate a URL-friendly slug
        public string UrlFriendly(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            // Convert to lowercase
            var slug = title.ToLowerInvariant();

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

            // Return the slug
            return slug;
        }

        // Generate context-aware slug
        public string GenerateSlug(string appName, string country, string? city = null)
        {
            var baseSlug = $"{appName}-{UrlFriendly(country)}";
            if (!string.IsNullOrWhiteSpace(city))
            {
                baseSlug += $"-{UrlFriendly(city)}";
            }
            return baseSlug;
        }
    }
}