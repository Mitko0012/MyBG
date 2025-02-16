using System.ComponentModel.DataAnnotations;
using System.Composition.Convention;

namespace MyBG.Models
{
    public class ApproveModel
    {
        [Required]
        public string MessageForApproved { get; set; }
        [Required]
        public string MessageForDeclined { get; set; }
        public EditModel EditModel { get; set; }
    }
}
