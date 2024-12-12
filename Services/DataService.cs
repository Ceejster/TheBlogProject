using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Enums;
using TheBlogProject.Models;

namespace TheBlogProject.Services
{
    public class DataService
    {
        //seed a user into the system
        //seed a a few roles into the system
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task ManageDataAsync()
        {
            await _dbContext.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task SeedRolesAsync()
        {
            //if there are already roles in system do nothing
            if (_dbContext.Roles.Any())
            {
                return;
            }

            //if nothing, create roles
            foreach (var role in Enum.GetNames(typeof(BlogRoles)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

        }

        private async Task SeedUsersAsync()
        {
            if (await _userManager.FindByEmailAsync("CJBowman@live.com") != null)
            {
                Console.WriteLine("Admin user already exists. Skipping user seeding.");
                return;
            }

            var adminUser = new BlogUser()
            {
                Email = "CJBowman@live.com",
                UserName = "CJBowman@live.com",
                FirstName = "CJ",
                LastName = "Bowman",
                DisplayName = "Ceej",
                PhoneNumber = "307-690-7593",
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(adminUser, "Passpass123!");
            
            await _userManager.AddToRoleAsync(adminUser, BlogRoles.Administrator.ToString());
           

            //can create modUser if want to
        }

    }
}
