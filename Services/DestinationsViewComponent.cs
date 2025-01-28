using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;

namespace TheBlogProject.Services
{
    public class DestinationsViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public DestinationsViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var destinations = await _context.Destination.ToListAsync();
            return View(destinations);
        }

    }
}
