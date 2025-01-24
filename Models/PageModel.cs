using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models
{
    public class PageModel
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? TextBody { get; set; }
        [Required]
        public string? Summary { get; set; }
        [NotMapped]
        [Required]
        public IFormFile? PageImage { get; set; }
        public byte[]? PageImageArr { get; set; }
        string? _returnFile;
        public bool Approved { get; set; } = false;
        public string? ReturnFile
        {
            get
            {
                try
                {
                    var base64 = Convert.ToBase64String(this.PageImageArr);

                    return string.Format("data:image/jpg;base64,{0}", base64);
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                _returnFile = value;
            }
        }
    }
}
