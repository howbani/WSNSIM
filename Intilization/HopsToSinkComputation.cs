using MiniSDN.Dataplane;
using MiniSDN.Dataplane.PacketRouter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSDN.Intilization
{
   public class HopsToSinkComputation
    {
        static Queue<Sensor> queu = new Queue<Sensor>();
        public static void ComputeHopsToSink(Sensor sinkNode)
        {

            // start from the sink.
            if (sinkNode.ID == Dataplane.PublicParamerters.SinkNode.ID)
            {
                sinkNode.HopsToSink = 0;
                sinkNode.lbl_hops_to_sink.Content = "0";
                NetworkInitialization(sinkNode);
            }
        }

        private static bool IsInTheQuue(Sensor sx)
        {
            List<Sensor> senlist = queu.ToList<Sensor>();
            foreach(Sensor s in senlist)
            {
                if (s.ID == sx.ID) return true;
            }
            return false;
        }
        private static void NetworkInitialization(Sensor x)
        {
          
            x.trun = true;
            if (x.NeighborsTable  != null)
            {
                if (x.NeighborsTable.Count != 0)
                {
                    foreach (NeighborsTableEntry i in x.NeighborsTable)
                    { 
                        if (x.HopsToSink < i.H)
                        {
                            i.NeiNode.HopsToSink = x.HopsToSink + 1;
                            i.NeiNode.lbl_hops_to_sink.Content = i.H.ToString();
                        }
                        // to queue.
                        if (i.NeiNode.trun == false)
                        {
                            if(!IsInTheQuue(i.NeiNode)) queu.Enqueue(i.NeiNode);
                        }
                    }
                    while (queu.Count>0)
                    {
                        Sensor top = queu.Dequeue();
                        int topID = top.ID;
                        NetworkInitialization(top);
                    }
                }
            }


        }
         
        
    }
}
