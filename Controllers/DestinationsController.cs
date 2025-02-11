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
    public class DestinationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly UserManager<BlogUser> _userManager;
        private readonly ISanitizeService _sanitizeService;
        private readonly ISlugService _slugService;

        public DestinationsController(ApplicationDbContext context, IImageService imageService, UserManager<BlogUser> userManager, ISanitizeService sanitizeService, ISlugService slugService)
        {
            _context = context;
            _imageService = imageService;
            _userManager = userManager;
            _sanitizeService = sanitizeService;
            _slugService = slugService;
        }

        // GET: Destinations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Destination
                .Include(d => d.BlogUser)
                .ThenInclude(b => b.Blogs);
            var destinations = await applicationDbContext.ToListAsync();

            return View(destinations);
        }

        // GET: Destinations/Details/5
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return View("NotFound");
            }

            var destination = await _context.Destination
                .Include(d => d.Blogs)
                    .ThenInclude(b => b.BlogUser)
                .FirstOrDefaultAsync(d => d.Slug == slug);

            if (destination == null)
            {
                return View("NotFound");
            }

            return View(destination);
        }


        // GET: Destinations/Create
        public IActionResult Create()
        {
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Destinations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Area,Image")] Destination destination)
        {
            if (ModelState.IsValid)
            {
                // Generate the slug
                destination.Slug = _slugService.GenerateSlug(destination.Area);
                // Validate the slug
                var (isValid, errorMessage) = _slugService.ValidateSlug(destination.Slug, "destination");

                if (!isValid)
                {
                    ModelState.AddModelError("Slug", errorMessage);
                    ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", destination.BlogUserId);
                    return View(destination);
                }

                // Set the additional properties
                destination.BlogUserId = _userManager.GetUserId(User);
                destination.ImageData = await _imageService.EncodeImageAsync(destination.Image);
                destination.ContentType = _imageService.ContentType(destination.Image);

                // Add and save the destination to the database
                _context.Add(destination);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", destination.BlogUserId);
            return View(destination);
        }

        // GET: Destinations/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var destination = await _context.Destination.FindAsync(id);
            if (destination == null)
            {
                return View("NotFound");
            }
            return View(destination);
        }

        // POST: Destinations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Area,Image")] Destination destination, IFormFile? newImage)
        {
            if (id != destination.Id)
            {
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDestination = await _context.Destination.FindAsync(id);

                    if (existingDestination == null)
                    {
                        return View("NotFound");
                    }

                    //Regenerate the slug if Area has been changed
                    if (existingDestination.Area != destination.Area)
                    {
                        existingDestination.Slug = _slugService.GenerateSlug(destination.Area);

                        // Validate the new slug
                        var (isValid, errorMessage) = _slugService.ValidateSlug(existingDestination.Slug, "destination");
                        if (!isValid)
                        {
                            ModelState.AddModelError("Slug", errorMessage);
                            return View(destination);
                        }
                    }

                    // Update other properties
                    existingDestination.Area = destination.Area;

                    // Update Image if provided
                    if (newImage != null)
                    {
                        existingDestination.ImageData = await _imageService.EncodeImageAsync(newImage);
                        existingDestination.ContentType = _imageService.ContentType(newImage);
                    }

                    // Save changes to database
                    _context.Update(existingDestination);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DestinationExists(destination.Id))
                    {
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", destination.BlogUserId);
            return View(destination);
        }

        // GET: Destinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var destination = await _context.Destination
                .Include(d => d.BlogUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (destination == null)
            {
                return View("NotFound");
            }

            return View(destination);
        }

        // POST: Destinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var destination = await _context.Destination.FindAsync(id);

            // Check if the destination has associated blogs
            var hasBlogs = await _context.Blogs.AnyAsync(b => b.DestinationId == id);

            if (hasBlogs)
            {
                // Return an error message or redirect to a warning view
                ModelState.AddModelError("", "Cannot delete this destination because it is associated with blogs.");
                return View(destination); // Or redirect to another page
            }

            if (destination != null)
            {
                _context.Destination.Remove(destination);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool DestinationExists(int id)
        {
            return _context.Destination.Any(e => e.Id == id);
        }
    }
}