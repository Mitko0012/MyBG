using MyBG.Data;

namespace MyBG.Models
{
    public class TransportWay
    {
        public int ID { get; set; }
        public TransportWays TransportWayType { get; set; }
        public string TransportTime { get; set; }
        public string TransportOrigin { get; set; }
        public int PageKey { get; set; }
    }
}
