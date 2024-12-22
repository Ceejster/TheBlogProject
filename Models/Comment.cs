using System.ComponentModel.DataAnnotations;
using TheBlogProject.Enums;

namespace TheBlogProject.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public required string BlogUserId { get; set; }
        public required string ModeratorId { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 10)]
        [Display(Name = "Comment")]
        public required string Body { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [DataType(DataType.Date)]
        [Display(Name = "Updated Date")]
        public DateTime? Updated { get; set; }
        public DateTime? Moderated { get; set; }
        public DateTime? Deleted { get; set; }

        [StringLength(550, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 10)]
        [Display(Name = "Moderated Comment")]
        public string? ModeratedBody { get; set; }

        public ModerationType ModerationType { get; set; }

        //Navigation properties
        public required virtual Post Post { get; set; }
        public required virtual BlogUser BlogUser { get; set; }
        public required virtual BlogUser Moderator { get; set; }
    }
}
