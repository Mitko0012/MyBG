namespace MyBG.Models;

public class EditModel
{
    public int ID {get; set;}
    [Required]
    public string OldText {get; set;}
    [Required]
    public string NewText {get; set;}
    [Required]
    public PageModel PageToEdit {get; set;}
    [Required]
    public int PageModelKey {get; set;}
    public bool Approved {get; set;}
}
