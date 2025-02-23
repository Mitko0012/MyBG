using System.ComponentModel.DataAnnotations;

namespace MyBG.Models
{
    public class DeclinePostModel
    {
        [Required(ErrorMessage = "Please write a message")]
        public string DeclineMessage { get; set; }
        public ForumQuestion Post { get; set; }
    }
}
