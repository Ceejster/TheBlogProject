using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBlogProject.Models
{
    public class BlogUser : IdentityUser
    {
        [Required]
        [StringLength(20, ErrorMessage = "The {0} is too long.", MinimumLength = 2)]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} is too long.", MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} is too long.", MinimumLength = 2)]
        [Display(Name = "Display Name")]
        public required string DisplayName { get; set; }

        public byte[]? ImageData { get; set; }
        public string? ContentType { get; set; }

        public string? FacebookUrl { get; set; }
        public string? GithubUrl { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public virtual ICollection<Blog> Blogs { get; set; } = new HashSet<Blog>();
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

    }
}
