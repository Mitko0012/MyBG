using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MyBG.Data;

namespace MyBG.Models
{
    public class PageModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please write a title for your page")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Please write the text contents for your page")]
        public string? TextBody { get; set; }
        [Required(ErrorMessage = "Please write a short description of what your page is about")]
        public string? Summary { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Please upload an image for the page")]
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
        public string? Comment { get; set; }
        [NotMapped]
        public int CommenntsToDisplay { get; set; } = 10;
        [Required(ErrorMessage = "Please add at least one transport way")]
        public List<TransportWay> TransportWays { get; set; }
        [Required]
        public Regions Regions { get; set; } = Regions.Southwestern;
        public List<EditModel> Edits {get; set;} = new List<EditModel>();
        public double Lat { get; set; }
        public double Long { get; set; }
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
        [Required(ErrorMessage = "Please add at least one transport way")]
        public string VerifyTransport { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DestinationType DestinationType { get; set; } = DestinationType.Town;
        public CultureType CultureType { get; set; } = CultureType.Meal;
        public bool IsCulture {get; set;} = false;
        [NotMapped]
        public string Scroll;
        [NotMapped]
        public bool Saved = false;
        [NotMapped]
        public string GetDestinationType
        {
            get
            {
                switch((int)DestinationType)
                {
                    case(0):
                        return "Landmark";
                        break;
                    case(1):
                        return "Historical Site";
                        break;
                    case(2):
                        return "Nature Site";
                        break;
                    case(3):
                        return "Town";
                        break;
                    default:
                        return "";
                        break;
                }
            }
        }
        [NotMapped]
        public string ReplyString;
    }
}
