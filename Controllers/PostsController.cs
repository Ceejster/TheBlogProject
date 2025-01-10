using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.Services;

namespace TheBlogProject.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ISlugService _slugService;
        private readonly ISanitizeService _sanitizeService;
        private readonly BlogSearchService _blogSearchService;

        public PostsController(ApplicationDbContext context, IImageService imageService, UserManager<BlogUser> userManager, ISlugService slugService, ISanitizeService sanitizeService, BlogSearchService blogSearchService)
        {
            _context = context;
            _imageService = imageService;
            _slugService = slugService;
            _sanitizeService = sanitizeService;
            _userManager = userManager;
            _blogSearchService = blogSearchService;
        }

        // For users searching the blogs for key words. Reference the service for more insight
        public IActionResult SearchIndex(string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var posts = _blogSearchService.Search(searchTerm);
            
            return View(posts);
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser);
            var posts = await applicationDbContext.ToListAsync();
            return View(posts); // Pass posts to the view
        }

        // BlogPostIndex
        public async Task<IActionResult> BlogPostIndex(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var posts = _context.Posts.Where(p => p.Id == id);
            return View("Index", posts);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .Include(p => p.Tags)
                .Include(p => p.Comments)
                .ThenInclude(c => c.BlogUser)
                .FirstOrDefaultAsync(m => m.Slug == slug);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name");
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("BlogId,Title,Abstract,Content,ReadyStatus,Image")] Post post, List<string> tagValues)
        {
            if (ModelState.IsValid)
            {
                //Sanitizing the content since viewing it .Raw
                post.Content = _sanitizeService.Sanitize(post.Content) ?? string.Empty;
                post.Abstract = _sanitizeService.Sanitize(post.Abstract) ?? string.Empty;

                post.Created = DateTime.UtcNow;

                //Setting the author Id
                var authorId = _userManager.GetUserId(User);
                post.BlogUserId = authorId;

                post.ImageData = await _imageService.EncodeImageAsync(post.Image);
                post.ContentType = _imageService.ContentType(post.Image);

                //Create the slug and see if it's unique
                var slug = _slugService.UrlFriendly(post.Title);

                // Create a var to store whether an error has occured
                var validateError = false;

                if (string.IsNullOrEmpty(slug))
                {
                    validateError = true;
                    ModelState.AddModelError("Title", "There is no title provided.");
                }

                // Detect duplicate slugs
                else if (!_slugService.IsUnique(slug))
                {
                    validateError = true;
                    ModelState.AddModelError("Title", "The title provided is already being used in another post.");
                }

                if (validateError)
                {

                    ViewData["TagValues"] = string.Join(",", tagValues);
                    ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
                    return View(post);
                }

                post.Slug = slug;

                _context.Add(post);
                await _context.SaveChangesAsync();

                foreach (var tagText in tagValues)
                {
                    _context.Add(new Tag()
                    {
                        PostId = post.Id,
                        BlogUserId = authorId,
                        Text = tagText
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", post.BlogId);
            return View(post);
        }

        // GET: Posts/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", post.BlogId);
            ViewData["TagValues"] = string.Join(", ", post.Tags.Select(t => t.Text));
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BlogId,Title,Abstract,Content,ReadyStatus")] Post updatedPost, IFormFile? newImage, List<string> tagValues)
        {
            if (id != updatedPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.Posts.Include(p => p.Tags).FirstOrDefaultAsync(p => p.Id == updatedPost.Id);
                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    // Check if the title has changed
                    if (!string.Equals(existingPost.Title, updatedPost.Title, StringComparison.OrdinalIgnoreCase))
                    {
                        var newSlug = _slugService.UrlFriendly(updatedPost.Title);

                        if (!_slugService.IsUnique(newSlug))
                        {
                            ModelState.AddModelError("Title", "The title provided is already being used in another post.");
                            ViewData["TagValues"] = string.Join(",", tagValues);
                            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Name", existingPost.BlogId);
                            return View(existingPost);
                        }

                        existingPost.Title = updatedPost.Title;
                        existingPost.Slug = newSlug;
                    }

                    existingPost.Abstract = _sanitizeService.Sanitize(updatedPost.Abstract) ?? string.Empty;
                    existingPost.Content = _sanitizeService.Sanitize(updatedPost.Content) ?? string.Empty;
                    existingPost.ReadyStatus = updatedPost.ReadyStatus;
                    existingPost.Updated = DateTime.UtcNow;

                    if (newImage != null)
                    {
                        existingPost.ImageData = await _imageService.EncodeImageAsync(newImage);
                        existingPost.ContentType = _imageService.ContentType(newImage);
                    }

                    // Remove all Tags previously associated with the Post
                    _context.Tags.RemoveRange(existingPost.Tags);

                    // Add in the new Tags from the Edit form
                    foreach (var tagText in tagValues)
                    {
                        _context.Add(new Tag
                        {
                            PostId = existingPost.Id,
                            BlogUserId = existingPost.BlogUserId,
                            Text = tagText
                        });
                    }

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(updatedPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogId"] = new SelectList(_context.Blogs, "Id", "Description", updatedPost.BlogId);
            return View(updatedPost);
        }


        // GET: Posts/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Blog)
                .Include(p => p.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }
    }
}