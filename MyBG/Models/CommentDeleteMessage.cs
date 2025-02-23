using System.ComponentModel.DataAnnotations;

namespace MyBG.Models
{
    public class CommentDeleteMessage
    {
        [Required(ErrorMessage = "Please write a message")]
        public string Message { get; set; } = "";
        public int Id { get; set; }
    }
}
