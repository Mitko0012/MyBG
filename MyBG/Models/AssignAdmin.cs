using System.ComponentModel.DataAnnotations;

namespace MyBG.Models;

public class AssignAdmin
{
    public string User = "";
    [Required(ErrorMessage = "Please write a message")]
    public string Message {get; set;}
}