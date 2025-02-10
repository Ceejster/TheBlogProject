using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlogProject.Models
{
    public class Blog
    {
        public int Id { get; set; }

        //Will need to add destination ID?
        [Display(Name = "Destination or area")]
        public int? DestinationId { get; set; }

        [Display(Name = "Name")]
        public string? BlogUserId { get; set; }

        [Required(ErrorMessage = "Country name is required.")]
        [Display(Name = "Country")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "{0} is too long or too short. Must be between {2} and {1}.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "Add some more detail but stay below {2}.")]
        public required string Description { get; set; }
        public required string Details { get; set; }
        [Range(0, 365, ErrorMessage = "Time must be a positive number.")]
        public required double Time { get; set; }
        public required string TimeUnit { get; set; } // Stores "Days", "Weeks", "Months", or "Years"

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        public string? Slug { get; set; }

        [Display(Name = "Blog Image")]
        public byte[]? ImageData { get; set; }

        [Display(Name = "Image Type")]
        public string? ContentType { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        //Navigation Property
        public virtual Destination? Destination { get; set; }

        [Display(Name = "Blogthor")]
        public virtual BlogUser? BlogUser { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
    }
}
