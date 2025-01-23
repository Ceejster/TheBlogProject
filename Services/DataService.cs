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
            // Admin User
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

            if (await _userManager.FindByEmailAsync(adminUser.Email) == null)
            {
                var adminResult = await _userManager.CreateAsync(adminUser, "Passpass123!");
                if (!adminResult.Succeeded)
                {
                    throw new Exception($"Failed to create admin user: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");
                }
                await _userManager.AddToRoleAsync(adminUser, BlogRoles.Administrator.ToString());
            }

            // Moderator User
            var moderatorUser = new BlogUser()
            {
                Email = "craigjessie11@gmail.com",
                UserName = "craigjessie11@gmail.com",
                FirstName = "Craig Jessie",
                LastName = "Bowman",
                DisplayName = "Ceejay",
                PhoneNumber = "307-690-7593",
                EmailConfirmed = true
            };

            if (await _userManager.FindByEmailAsync(moderatorUser.Email) == null)
            {
                var moderatorResult = await _userManager.CreateAsync(moderatorUser, "thisPassword123!");
                if (!moderatorResult.Succeeded)
                {
                    throw new Exception($"Failed to create moderator user: {string.Join(", ", moderatorResult.Errors.Select(e => e.Description))}");
                }
                await _userManager.AddToRoleAsync(moderatorUser, BlogRoles.Moderator.ToString());
            }
        }


    }
}
