using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models
{
    public class ForumQuestion
    {
        public int Id { get; set; } 
        [Required]
        public string Title { get; set; }
        [Required] 
        public string Text { get; set; }
        public List<PFPModel> LikedUser { get; set; } = new List<PFPModel>();
        public List<CommentModel> Comment { get; set; } = new List<CommentModel>();
        [Required]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; }
        public int LikesReturn
        {
            get
            {
                return LikedUser.Count;
            }
        }
        [NotMapped]
        public String CommentCurrent { get; set; } = "";
        [NotMapped]
        public int CommentCount { get; set; } = 10;
        public bool IsDeleted { get; set; } = false;
        [Required]
        public IdentityUser User { get; set; }
        [Required]
        public string? ForeignKey { get; set; } = "User1";
        [NotMapped]
        public PFPModel Pfp { get; set; }
    }
}
