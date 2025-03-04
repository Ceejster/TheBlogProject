using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;

namespace TheBlogProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogEmailSender _emailSender;

        public HomeController(ApplicationDbContext context, IBlogEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _context.Blogs
                .Include(b => b.BlogUser)
                .Include(b => b.Posts) // Load posts for each blog
                .ToListAsync();

            // Generic SEO data for the home page
            ViewData["Title"] = "Our Life in Travel";
            ViewData["MetaDescription"] = "Discover travel experiences, tips, and adventures that my wife and I do around the world.";
            ViewData["CanonicalUrl"] = Url.Action("Index", "Home", null, Request.Scheme);
            // Optionally, you might set an ImageUrl for social sharing if it applies to the home page
            //ViewData["ImageUrl"] = _imageService.DecodeImage(blog.ImageData, blog.ContentType);

            return View(blogs); // Pass the blogs with posts to the view
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactMe model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var subject = "New Contact Form Submission";
            var plainTextContent = $"Message from {model.Name} ({model.Email}): {model.Message} <br>{model.Phone}";
            var htmlContent = $"<strong>Message from {model.Name} ({model.Email}):</strong><br>{model.Message}";

            await _emailSender.SendEmailAsync(model.Email, subject, plainTextContent, htmlContent);

            ViewBag.Message = "Your message has been sent!";

            //CHANGE THIS TO A THANKS FOR CONTACTING ME PAGE
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
