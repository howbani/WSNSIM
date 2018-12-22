using MiniSDN.Intilization;
namespace MiniSDN.Dataplane.NOS
{
    public enum PacketType { Beacon, Preamble, ACK, Data, Control }
    public class Packet
    {
        //: Packet section:
        public long PID { get; set; } // SEQ ID OF PACKET.
        public PacketType PacketType { get; set; }
        public bool isDelivered { get; set; }
        public double PacketLength { get; set; }
        public int H2S { get { if (PacketType == PacketType.Data) return Source.HopsToSink; else return Destination.HopsToSink; } }
        public int TimeToLive { get; set; }
        public int Hops { get; set; }
        public string Path { get; set; }
        public double RoutingDistance { get; set; }
        public double Delay { get; set; }
        public double UsedEnergy_Joule { get; set; }
        public int WaitingTimes { get; set; }

        public double EuclideanDistance
        {
            get { return Operations.DistanceBetweenTwoSensors(Source, Destination); }
        }

        /// <summary>
        /// eff 100%
        /// </summary>
        public double RoutingDistanceEfficiency
        {
            get
            {
                return 100 * (EuclideanDistance / RoutingDistance);
            }
        }

        /// <summary>
        /// Average Transmission Distance (ATD): for〖 P〗_b^s (g_k ), we define average transmission distance per hop as shown in (28).
        /// </summary>
        public double AverageTransDistrancePerHop
        {
            get
            {
                return (RoutingDistance / Hops);
            }
        }


        public double TransDistanceEfficiency
        {
            get
            {
                return 100 * (1 - (RoutingDistance / (PublicParamerters.CommunicationRangeRadius * Hops * (Hops + 1))));
            }
        }


        /// <summary>
        /// RoutingEfficiency
        /// </summary>
        public double RoutingEfficiency
        {
            get
            {
                return (RoutingDistanceEfficiency + TransDistanceEfficiency) / 2;
            }
        }

        public Sensor Source { get; set; }
        public Sensor Destination { get; set; }
    }
}
