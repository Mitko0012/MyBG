using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        public IdentityUser User { get; set; }  
        [NotMapped]
        public PFPModel PFP { get; set; }
        public List<PageModel> Pages { get; set; } = new List<PageModel>();
        public List<PFPModel> LikedUser { get; set; } = new List<PFPModel>();
        public List<ForumQuestion> PostedOnForums { get; set; } = new List<ForumQuestion>();
        public int? PageId { get; set; }
        public int? PostId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
