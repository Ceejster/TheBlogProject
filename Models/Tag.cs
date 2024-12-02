using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TheBlogProject.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public required string BlogUserId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        public required string Text { get; set; }

        public required virtual Post Post { get; set; }
        public required virtual BlogUser BlogUser { get; set; }
    }
}
