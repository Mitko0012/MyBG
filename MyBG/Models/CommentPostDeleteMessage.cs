using System.ComponentModel.DataAnnotations;

namespace MyBG.Models
{
    public class CommentPostDeleteMessage
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please write your message")]
        public string Message {  get; set; } = "";
    }
}
