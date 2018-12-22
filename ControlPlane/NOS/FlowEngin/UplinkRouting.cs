using MiniSDN.Dataplane;
using MiniSDN.Dataplane.PacketRouter;
using MiniSDN.Intilization;
using MiniSDN.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniSDN.ControlPlane.NOS.FlowEngin
{
    
    public class MiniFlowTableSorterUpLinkPriority : IComparer<MiniFlowTableEntry>
    {

        public int Compare(MiniFlowTableEntry y, MiniFlowTableEntry x)
        {
            return x.UpLinkPriority.CompareTo(y.UpLinkPriority);
        }
    }

    public class UplinkFlowEnery
    {

        public int CurrentID { get { return Current.ID; } } // ID
        public int NextID { get { return Next.ID; } }
        //
        public double Pr
        {
            get; set;
        }

        // Elementry values:
        public double H { get; set; } // hop to sink
        public double R { get; set; } // riss
        public double L { get; set; } // remian energy
        //
        public double HN { get; set; } // H normalized
        public double RN { get; set; } // R NORMALIZEE value of To. 
        public double LN { get; set; } // L normalized
        //
        public double HP { get; set; } // R normalized
        public double RP { get; set; } // R NORMALIZEE value of To. 
        public double LP { get; set; } // L value of To.



        // return:
        public double Mul
        {
            get
            {
                return RP * LP * HP;
            }
        }

        public Sensor Current { get; set; } // ID
        public Sensor Next { get; set; }
    }

    public class UplinkRouting
    {

        public static void UpdateUplinkFlowEnery(Sensor sender)
        {
            sender.GenerateDataPacket(); // send packet to controller.
            PublicParamerters.SinkNode.GenerateControlPacket(sender);// response from controller.

            sender.MiniFlowTable.Clear();
            ComputeUplinkFlowEnery(sender);

          
        }

        public static void ComputeUplinkFlowEnery(Sensor sender)
        {


            double n =  Convert.ToDouble(sender.NeighborsTable.Count) + 1;

            double LControl = Settings.Default.ExpoLCnt * Math.Sqrt(n);
            double HControl = Settings.Default.ExpoHCnt * Math.Sqrt(n);
            double EControl = Settings.Default.ExpoECnt * Math.Sqrt(n);
            double RControl = Settings.Default.ExpoRCnt * Math.Sqrt(n);


            double HSum = 0; // sum of h value.
            double RSum = 0;
            foreach (NeighborsTableEntry can in sender.NeighborsTable)
            {
                HSum += can.H;
                RSum += can.R;
            }

            // normalized values.
            foreach (NeighborsTableEntry neiEntry in sender.NeighborsTable)
            {
                if (neiEntry.NeiNode.ResidualEnergyPercentage >= 0) // the node is a live.
                {
                    MiniFlowTableEntry MiniEntry = new MiniFlowTableEntry();
                    MiniEntry.NeighborEntry = neiEntry;
                    MiniEntry.NeighborEntry.HN = 1.0 / (Math.Pow((Convert.ToDouble(MiniEntry.NeighborEntry.H) + 1.0), HControl));
                    MiniEntry.NeighborEntry.RN = 1 - (Math.Pow(MiniEntry.NeighborEntry.R, RControl) / RSum);
                    MiniEntry.NeighborEntry.LN = Math.Pow(MiniEntry.NeighborEntry.L / 100, LControl);

                    MiniEntry.NeighborEntry.E = Operations.DistanceBetweenTwoPoints(PublicParamerters.SinkNode.CenterLocation, MiniEntry.NeighborEntry.CenterLocation);
                    MiniEntry.NeighborEntry.EN = (MiniEntry.NeighborEntry.E / (Operations.DistanceBetweenTwoPoints(PublicParamerters.SinkNode.CenterLocation, sender.CenterLocation) + sender.ComunicationRangeRadius));

                    sender.MiniFlowTable.Add(MiniEntry);
                }
            }

            // pro sum
            double HpSum = 0; // sum of h value.
            double LpSum = 0;
            double RpSum = 0;
            double EpSum = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                HpSum += (1 - Math.Exp(MiniEntry.NeighborEntry.HN));
                RpSum += Math.Exp(MiniEntry.NeighborEntry.RN);
                LpSum += (1 - Math.Exp(-MiniEntry.NeighborEntry.LN));
                EpSum += (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.EN)), EControl));
            }

            double sumAll = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.NeighborEntry.HP = (1 - Math.Exp(MiniEntry.NeighborEntry.HN)) / HpSum;
                MiniEntry.NeighborEntry.RP = Math.Exp(MiniEntry.NeighborEntry.RN) / RpSum;
                MiniEntry.NeighborEntry.LP = (1 - Math.Exp(-MiniEntry.NeighborEntry.LN)) / LpSum;
                MiniEntry.NeighborEntry.EP = (Math.Pow((1 - Math.Sqrt(MiniEntry.NeighborEntry.EN)), EControl)) / EpSum;

                MiniEntry.UpLinkPriority = (MiniEntry.NeighborEntry.EP + MiniEntry.NeighborEntry.HP + MiniEntry.NeighborEntry.RP + MiniEntry.NeighborEntry.LP) / 4;
                sumAll += MiniEntry.UpLinkPriority;
            }

            // normalized:
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                MiniEntry.UpLinkPriority = (MiniEntry.UpLinkPriority / sumAll);
            }
            // sort:
            sender.MiniFlowTable.Sort(new MiniFlowTableSorterUpLinkPriority());

            // action:
            double average = 1 / Convert.ToDouble(sender.MiniFlowTable.Count);
            int Ftheashoeld = Convert.ToInt16(Math.Ceiling(Math.Sqrt(Math.Sqrt(n)))); // theshold.
            int forwardersCount = 0;
            foreach (MiniFlowTableEntry MiniEntry in sender.MiniFlowTable)
            {
                if (MiniEntry.UpLinkPriority >= average && forwardersCount <= Ftheashoeld)
                {
                    MiniEntry.UpLinkAction = FlowAction.Forward;
                    forwardersCount++;
                }
                else MiniEntry.UpLinkAction = FlowAction.Drop;
            }
        }
    }
}
