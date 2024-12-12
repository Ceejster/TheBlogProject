using Microsoft.AspNetCore.Mvc;
using TheBlogProject.Services;

namespace TheBlogProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IBlogEmailSender _emailSender;
        public AccountController(IBlogEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<IActionResult> SendConfirmationEmail(string userEmail)
        {
            var subject = "Confirm Your Email";
            var plainTextContent = "Please confirm your email by clicking the link.";
            var htmlContent = "<strong>Please confirm your email by clicking the link.</strong>";
            await _emailSender.SendEmailAsync(userEmail, subject, plainTextContent, htmlContent);
            return View();
        }

    }
}
