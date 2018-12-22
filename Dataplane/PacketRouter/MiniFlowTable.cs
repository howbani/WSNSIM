using MiniSDN.Dataplane.NOS;

namespace MiniSDN.Dataplane.PacketRouter
{
    public enum FlowAction { Forward, Drop }

    public class MiniFlowTableEntry
    {
        public int ID { get { return NeighborEntry.NeiNode.ID; } }
        public double UpLinkPriority { get; set; }
        public FlowAction UpLinkAction { get; set; }
        public double UpLinkStatistics { get; set; }  

        public double DownLinkPriority { get; set; }
        public FlowAction DownLinkAction { get; set; }
        public double DownLinkStatistics { get; set; }

        public SensorState SensorState { get { return NeighborEntry.NeiNode.CurrentSensorState; } }
        public double Statistics { get { return UpLinkStatistics + DownLinkStatistics; } }
        public  NeighborsTableEntry NeighborEntry { get; set; } 

    }
}
