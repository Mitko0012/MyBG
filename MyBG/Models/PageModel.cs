using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyBG.Data;

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
        public List<PFPModel> UsersLiked { get; set; } = [];
        string? _returnFile;
        public bool Approved { get; set; } = false;
        [NotMapped]
        public bool LikedByUser { get; set; }
        public List<CommentModel> Comments { get; set; } = new List<CommentModel>();
        public int CommentId { get; set; }
        [NotMapped]
        public string? Comment { get; set; } = "";
        [NotMapped]
        public int CommenntsToDisplay { get; set; } = 10;
        [Required]
        public List<TransportWay> TransportWays { get; set; }
        [Required]
        public Regions Regions { get; set; } = Regions.Southwestern;
        public List<EditModel> Edits {get; set;} = new List<EditModel>();
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
