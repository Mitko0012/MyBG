using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models
{
    public class PFPModel
    {
        public int Id { get; set; }
        public string? UserName {  get; set; }
        [NotMapped]
        public IFormFile FormFile { get; set; }
        public byte[]? Image { get; set; } = [0];
        string _returnFile;
        public List<PageModel> PagesLiked { get; set; } = [];
        public List<CommentModel> CommentsLiked { get; set; } = new List<CommentModel>();
        public List<ForumQuestion> UpvotedForums { get; set; } = new List<ForumQuestion>();
        public List<EditModel> Contributions { get; set; } = new List<EditModel>();
        public int PageLikeId { get; set; }
        public string? ReturnFile
        {
            get
            {
                try
                {
                    var base64 = Convert.ToBase64String(this.Image);

                    return Image == null? "" : string.Format("data:image/jpg;base64,{0}", base64);
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
