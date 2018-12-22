using MiniSDN.Dataplane;
using MiniSDN.Dataplane.NOS;
using MiniSDN.Dataplane.PacketRouter;
using MiniSDN.Intilization;
using MiniSDN.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSDN.ControlPlane.NOS.FlowEngin
{
    public class MiniFlowTableSorterDownLinkPriority : IComparer<MiniFlowTableEntry>
    {

        public int Compare(MiniFlowTableEntry y, MiniFlowTableEntry x)
        {
            return x.DownLinkPriority.CompareTo(y.DownLinkPriority);
        } 
    } 
    public class DownlinkFlowEnery
    {
        public Sensor Current { get; set; }
        public Sensor Next { get; set; }
        public Sensor Target { get; set; }
        // Elementry values:
        public double D { get; set; } // direction value tworads the end node
        public double DN { get; set; } // R NORMALIZEE value of To. 
        public double DP { get; set; } // defual.

        public double L { get; set; } // remian energy
        public double LN { get; set; } // L normalized
        public double LP { get; set; } // L value of To.

        public double R { get; set; } // riss
        public double RN { get; set; } // R NORMALIZEE value of To. 
        public double RP { get; set; } // R NORMALIZEE value of To. 
        //
        public double Pr
        {
            get; set;
        }

        // return:
        public double Mul
        {
            get
            {
                return LP * DP * RP;
            }
        }

        public int IindexInMiniFlow { get; set; }
        public MiniFlowTableEntry MiniFlowTableEntry { get; set; }
    }

    public class DownLinkRouting
    {
       
        /// <summary>
        /// This will be change per sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="endNode"></param>
        public static void  GetD_Distribution(Sensor sender, Sensor endNode)
        {
            double n = Convert.ToDouble(sender.NeighborsTable.Count) + 1;

            double Dcontrol = Settings.Default.ExpoDCnt * Math.Sqrt(n);

            // normalized values.
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.NeighborEntry.D = Operations.DistanceBetweenTwoPoints(endNode.CenterLocation, MiniEntry.NeighborEntry.CenterLocation);
                MiniEntry.NeighborEntry.DN = (MiniEntry.NeighborEntry.D / (Operations.DistanceBetweenTwoPoints(endNode.CenterLocation, sender.CenterLocation) + sender.ComunicationRangeRadius));
            }

            // pro sum
            double DpSum = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                DpSum += (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.DN)), 1 + Dcontrol));
            }

            double sumAll = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.NeighborEntry.DP = (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.DN)), 1 + Dcontrol)) / DpSum;

                //: 
                MiniEntry.DownLinkPriority = (MiniEntry.NeighborEntry.DP + MiniEntry.NeighborEntry.LP + MiniEntry.NeighborEntry.RP) / 3;
                sumAll += MiniEntry.DownLinkPriority;
            }

            // normlizd
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.DownLinkPriority = (MiniEntry.DownLinkPriority / sumAll);
            }

            // sort:

            MiniFlowTableSorterDownLinkPriority xxxx = new MiniFlowTableSorterDownLinkPriority();
            sender.MiniFlowTable.Sort(xxxx);

            double average = 1 / Convert.ToDouble(sender.MiniFlowTable.Count);
            int Ftheashoeld = Convert.ToInt16(Math.Ceiling(Math.Sqrt(Math.Sqrt(n)))); // theshold.
            int forwardersCount = 0;

            // action:
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                if (MiniEntry.DownLinkPriority >= average && forwardersCount <= Ftheashoeld)
                {
                    MiniEntry.DownLinkAction = FlowAction.Forward;
                    forwardersCount++;
                }
                else MiniEntry.DownLinkAction = FlowAction.Drop;
            }

        }
    }
}
