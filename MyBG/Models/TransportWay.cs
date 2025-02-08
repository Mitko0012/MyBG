using MyBG.Data;

namespace MyBG.Models
{
    public class TransportWay
    {
        public int ID { get; set; }
        public TransportWays TransportWayType { get; set; } = new TransportWays();
        public string TransportTime { get; set; }
        public string TransportOrigin { get; set; }
    }
}
