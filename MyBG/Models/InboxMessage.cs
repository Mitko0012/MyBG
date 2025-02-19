using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MyBG.Models
{
    public class InboxMessage
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title {get; set;} = "";
        [Required]
        public string Message { get; set; }
        [Required]
        public PFPModel UserSource { get; set; }   
    }
}
