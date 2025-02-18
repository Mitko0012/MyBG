using MyBG.Data;

namespace MyBG.Models
{
    public class TransportWay
    {
        public int ID { get; set; }
        public TransportWays TransportWayType { get; set; } = new TransportWays();
        public int TransportTimeHours { get; set; }
        public int TransportTimeMinutes { get; set; }
        public string TransportOrigin { get; set; }
    }
}
