using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly UserManager<BlogUser> userManager;

        public BlogsController(ApplicationDbContext context, IImageService imageService, UserManager<BlogUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            this.userManager = userManager;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Blogs.Include(b => b.BlogUser);
            var blogs = await applicationDbContext.ToListAsync();
            return View(blogs); // Pass blogs to the view
        }


        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Blogs/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Image")] Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.BlogUserId = userManager.GetUserId(User);
                blog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                blog.ContentType = _imageService.ContentType(blog.Image);

                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", blog.BlogUserId);
            return View(blog);
        }

        // GET: Blogs/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Image")] Blog blog)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBlog = await _context.Blogs.FindAsync(id);
                    if (existingBlog == null)
                    {
                        return NotFound();
                    }

                    // Update properties
                    existingBlog.Name = blog.Name;
                    existingBlog.Description = blog.Description;
                    existingBlog.Updated = DateTime.UtcNow;

                    // Update Image if provided
                    if (blog.Image != null)
                    {
                        existingBlog.ImageData = await _imageService.EncodeImageAsync(blog.Image);
                        existingBlog.ContentType = _imageService.ContentType(blog.Image);
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