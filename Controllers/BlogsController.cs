using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.Services;

namespace TheBlogProject.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ISanitizeService _sanitizeService;
        private readonly ISlugService _slugService;

        public BlogsController(ApplicationDbContext context, IImageService imageService, UserManager<BlogUser> userManager, ISanitizeService sanitizeService, ISlugService slugService)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _sanitizeService = sanitizeService;
            _slugService = slugService;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Blogs
                .Include(b => b.BlogUser)
                .Include(p => p.Posts);
            var blogs = await applicationDbContext.ToListAsync();
            return View(blogs); // Pass blogs to the view
        }

        public async Task<IActionResult> BlogIndex(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogs = _context.Blogs
                .Where(b => b.DestinationId == id)
                .Include(b => b.BlogUser);
            return View("Index", blogs);
        }


        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(d => d.Posts)
                    .ThenInclude(b => b.BlogUser)
                .FirstOrDefaultAsync(d => d.Slug == slug);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            ViewData["DestinationId"] = new SelectList(_context.Destination, "Id", "Area");
            return View();
        }

        // POST: Blogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Moderator")]
        public async Task<IActionResult> Create([Bind("DestinationId,Name,Description,Details,Time,TimeUnit,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                // Generate the slug
                blog.Slug = _slugService.GenerateSlug(blog.Name);
                // Validate the sug
                var (isValid, errorMessage) = _slugService.ValidateSlug(blog.Slug, "destination");

                if (!isValid)
                {
                    ModelState.AddModelError("Slug", errorMessage);
                    ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
                    return View(blog);
                }

                //Sanitize raw HTML
                blog.Description = _sanitizeService.Sanitize(blog.Description) ?? string.Empty;
                blog.Details = _sanitizeService.Sanitize(blog.Details) ?? string.Empty;

                blog.BlogUserId = _userManager.GetUserId(User);
                blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                blog.ContentType = _imageService.ContentType(blog.Image);


                // Add and save to the database
                _context.Add(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
            return View(blog);
        }

        // GET: Blogs/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Details,Time,TimeUnit")] Blog blog, IFormFile? newImage)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Sanitize raw HTML
                    blog.Description = _sanitizeService.Sanitize(blog.Description) ?? string.Empty;
                    blog.Details = _sanitizeService.Sanitize(blog.Details) ?? string.Empty;

                    var existingBlog = await _context.Blogs.FindAsync(id);
                    if (existingBlog == null)
                    {
                        return NotFound();
                    }

                    //Regenerate the slug if Name has been changed
                    if (existingBlog.Name != blog.Name)
                    {
                        existingBlog.Slug = _slugService.GenerateSlug(existingBlog.Name);

                        // Validate the new slug
                        var (isValid, errorMessage) = _slugService.ValidateSlug(existingBlog.Slug, "blog");
                        if (!isValid)
                        {
                            ModelState.AddModelError("Slug", errorMessage);
                            return View(blog);
                        }
                    }

                    // Update properties
                    existingBlog.Name = blog.Name;
                    existingBlog.Description = blog.Description;
                    existingBlog.Details = blog.Details;
                    existingBlog.Time = blog.Time;
                    existingBlog.TimeUnit = blog.TimeUnit;
                    existingBlog.Updated = DateTime.UtcNow;

                    // Update Image if provided
                    if (newImage != null)
                    {
                        existingBlog.ImageData = await _imageService.EncodeImageAsync(newImage);
                        existingBlog.ContentType = _imageService.ContentType(newImage);
                    }

                    _context.Update(existingBlog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
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
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
            return View(blog);
        }


        // GET: Blogs/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs
                .Include(b => b.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                _context.Blogs.Remove(blog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}