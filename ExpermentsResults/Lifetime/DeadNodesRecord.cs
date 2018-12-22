using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSDN.ExpermentsResults.Lifetime 
{
    public class DeadNodesRecord
    {
        public double RoutingZone { get; set; }
        public int NOS { get; set; } // NUMBER OF RANDOM SELECTED SOURCES
        public int NOP { get; set; } // NUMBER OF PACKETS TO BE SEND.
        public long Rounds { get; set; }
        public int DeadNodeID { get; set; }
        public long DeadAfterPackets { get; set; } // deade after sending xx packets. the whole number of packets.
        public int DeadOrder { get; set; } // ترتيب الوفاه في الشبككه . ماتت رقم كم؟
    }
}
