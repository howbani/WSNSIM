using System;
using System.Collections.Generic;
using MiniSDN.Dataplane;
using MiniSDN.Properties;
using MiniSDN.Forwarding;
using System.Threading;

namespace MiniSDN.ExpermentsResults.Lifetime
{
    public class NetworkLifeTime
    {

        /// <summary>
        /// how many sensor to be slected.
        /// how many packet each sensor will sent.
        /// </summary>
        /// <param name="NOS"></param>
        /// <param name="NOP"></param>
        public void RandimSelect(List<Sensor> Network, int NOS, int NOP)  
        {
            // selecte The Nodes:
            List<Sensor> SelectedSn = new List<Dataplane.Sensor>(NOS);
            for (int i = 0; i < NOS; i++)
            {
                int ran = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(Network.Count - 2));
                SelectedSn.Add(Network[ran]);
            }

            // each packet sendt NOP:
            for (int i = 0; i < NOP; i++)
            {
                foreach (Sensor sen in SelectedSn)
                {
                    sen.GenerateDataPacket();
                }
            }

        } // end class random generated.



        public void FromAllNodes(List<Sensor> Network)
        {
            foreach (Sensor sen in Network)
            {
                sen.GenerateDataPacket();
                Thread.Sleep(TimeSpan.FromMilliseconds(10));
            }
        }


    }
}
