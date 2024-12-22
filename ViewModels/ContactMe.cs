using System.ComponentModel.DataAnnotations;

namespace TheBlogProject.ViewModels
{
    public class ContactMe
    {
        [Required]
        public required string Name { get; set; }

        [EmailAddress]
        [Required]
        public required string Email { get; set; }

        [Phone]

        [Display(Name = "WhatsApp")]
        public string? Phone { get; set; }

        [Required]
        public required string Subject { get; set; }

        [Required]
        public required string Message { get; set; }
    }
}
