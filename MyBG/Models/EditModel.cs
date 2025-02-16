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
    public int? PageIndex { get; set; } = 0;
    public bool CreatePage { get; set;}
    public int PFPKey { get; set; }
    public PFPModel? UserCreated { get; set;}
    public bool IsDeleted { get; set; } = false;
}
