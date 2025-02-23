using System.ComponentModel.DataAnnotations;
using System.Composition.Convention;

namespace MyBG.Models
{
    public class ApproveModel
    {
        [Required(ErrorMessage = "Please write a message")]
        public string MessageForApproved { get; set; }
        [Required(ErrorMessage = "Please write a message")]
        public string MessageForDeclined { get; set; }
        public EditModel EditModel { get; set; }
    }
}
