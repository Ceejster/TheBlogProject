using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Models;
using TheBlogProject.Services;
using TheBlogProject.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Bind MailSettings from configuration
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<DataService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Register your DataService
builder.Services.AddScoped<DataService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IBlogEmailSender, BlogEmailSender>();
builder.Services.AddScoped<BlogSearchService>();

// Register ImageService
builder.Services.AddScoped<IImageService, BasicImageService>();

//Register SluService
builder.Services.AddScoped<ISlugService, BasicSlugService>();

//Register Sanitization
builder.Services.AddScoped<ISanitizeService, SanitizationService>();

var app = builder.Build();

// Apply pending migrations and seed roles/users on startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbService = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dataService = scope.ServiceProvider.GetRequiredService<DataService>();

        // Apply pending migrations automatically
        dbService.Database.Migrate();

        // Seed roles and users
        await dataService.ManageDataAsync();
    }
    catch (Exception ex)
    {
        // Log exceptions (optional: replace with your logger)
        Console.WriteLine($"Error during migration or seeding: {ex.Message}");
        throw;
    }
}

// Add CSP Middleware to help protect from malicious code being injected.
// Resolve to allow bootstrap, JS, and CSS
//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Append("Content-Security-Policy",
//        "default-src 'self'; " +
//        "script-src 'self'; " +
//        "style-src 'self' 'unsafe-inline'; " +
//        "img-src 'self' data:;");
//    await next();
//});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// For better SEO 
app.MapControllerRoute(
    name: "destination",
    pattern: "destination/{slug}",
    defaults: new { controller = "Destinations", action = "Details" },
    constraints: new { slug = @"[\w\-]+" }
);

app.MapControllerRoute(
    name: "blog",
    pattern: "blog/{slug}",
    defaults: new { controller = "Blogs", action = "Details" },
    constraints: new { slug = @"[\w\-]+" }
);

app.MapControllerRoute(
    name: "post",
    pattern: "post/{slug}",
    defaults: new { controller = "Posts", action = "Details" },
    constraints: new { slug = @"[\w\-]+" }
);

// Default fallback route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//OG use if anything broken
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();