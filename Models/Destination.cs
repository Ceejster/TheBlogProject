using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlogProject.Models
{
    public class Destination
    {
        public int Id { get; set; }

        public string? BlogUserId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        [Display(Name = "Continent or area")]
        public required string Area { get; set; }
        public string? Slug { get; set; }

        [Display(Name = "Alternate Image Text")]
        public string Alt { get; set; }

        [Display(Name = "Blog Image")]
        public byte[]? ImageData { get; set; }

        [Display(Name = "Image Type")]
        public string? ContentType { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        //Navigation Property
        [Display(Name = "Blogthor")]
        public virtual BlogUser? BlogUser { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
    }
}
