using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheBlogProject.Enums;

namespace TheBlogProject.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Display(Name = "Blog Name")]
        public required int BlogId { get; set; }
        public string? BlogUserId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 3)]
        public required string Title { get; set; }

        [Required]
        [StringLength(250, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 10)]
        public required string Abstract { get; set; }

        [Required]
        public required string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime? Created { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }

        public required ReadyStatus ReadyStatus { get; set; }

        public string? Slug { get; set; }

        [Display(Name = "Post Image")]
        public byte[]? ImageData { get; set; }

        [Display(Name = "Image Type")]
        public string? ContentType { get; set; }

        [NotMapped]
        public IFormFile? Image { get; set; }

        //Navigation Properties
        public virtual Blog? Blog { get; set; }
        public virtual BlogUser? BlogUser { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
