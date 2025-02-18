using MyBG.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBG.Models
{
    public class PageModelContainer
    {
        public int Id { get; set; }
        public List<PageModel> Pages { get; set; }
        [NotMapped]
        public string DisplayType { get; set; }
        [NotMapped]
        public string SearchString { get; set; }
        public MyBG.Data.Regions RegionSelect { get; set; }
        public MyBG.Data.DestinationType DestinationTypeSelect { get; set; }
    }
}
