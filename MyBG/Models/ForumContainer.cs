using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models
{
    public class ForumContainer
    {
        public List<ForumQuestion> ForumPosts { get; set; }
        [NotMapped]
        public string SearchType { get; set; }
        public string SearchString { get; set; }
    }
}
