using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models;

public class EditModel
{
    public int ID {get; set;}
    public string? OldText {get; set;}
    [Required]
    public string NewText {get; set;}
    public PageModel? PageToEdit {get; set;}
    [Required]
    public int PageModelKey {get; set;}
    public bool Approved {get; set;}
    [NotMapped]
    public int? PageIndex { get; set;}
}
