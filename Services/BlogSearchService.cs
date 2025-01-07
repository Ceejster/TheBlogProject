using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services
{
    public class BlogSearchService
    {
        private readonly ApplicationDbContext _context;

        public BlogSearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Post> Search(string searchTerm)
        {
            // Only want ProductionReady blogs to appear in the search for now. 
            var posts = _context.Posts
                .Where(p => p.ReadyStatus == ReadyStatus.ProductionReady).AsQueryable();
            if (searchTerm != null)
            {
                // Using EF to make the loading and search times faster instead of everything being done on the users memory.
                posts = posts.Where(p =>
                EF.Functions.Like(p.Title.ToLower(), $"%{searchTerm.ToLower()}%") ||
                EF.Functions.Like(p.Abstract.ToLower(), $"%{searchTerm.ToLower()}%") ||
                EF.Functions.Like(p.Content.ToLower(), $"%{searchTerm.ToLower()}%") ||
                p.Comments.Any(c =>
                    EF.Functions.Like(c.Body.ToLower(), $"%{searchTerm.ToLower()}%") ||
                    EF.Functions.Like(c.ModeratedBody.ToLower(), $"%{searchTerm.ToLower()}%") ||
                    EF.Functions.Like(c.BlogUser.FirstName.ToLower(), $"%{searchTerm.ToLower()}%") ||
                    EF.Functions.Like(c.BlogUser.LastName.ToLower(), $"%{searchTerm.ToLower()}%") ||
                    EF.Functions.Like(c.BlogUser.Email.ToLower(), $"%{searchTerm.ToLower()}%")
                ));

            }
            return posts.OrderByDescending(p => p.Created);
        }
    }
}
