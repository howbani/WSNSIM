using MiniSDN.ControlPlane.NOS;
using MiniSDN.ControlPlane.NOS.FlowEngin;
using MiniSDN.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSDN.Charts.Intilization 
{
   public class DistrubtionsTests
    {
        /// <summary>
        ///  en.H = (i * Hpiovot);
        /// </summary>
        /// <param name="neigCount"></param>
        /// <param name="Hpiovot"></param>
        /// <returns></returns>
        public static List<UplinkFlowEnery> TestHvalue(int neigCount, int Hpiovot) 
        {
            List<UplinkFlowEnery> table = new List<UplinkFlowEnery>();
            // normalized values.

            for (int i= 1; i<= neigCount; i++)
            {
                UplinkFlowEnery en = new UplinkFlowEnery();
                en.Current = new Dataplane.Sensor(0);
                en.Next = new Dataplane.Sensor(1);
                en.H = (i * Hpiovot);
                en.HN = 1.0 / (Math.Pow((Convert.ToDouble(en.H) + 1.0), 1 + Settings.Default.ExpoHCnt));
                table.Add(en);
            }

            // pro sum
            double HpSum = 0; // sum of h value.

            foreach (UplinkFlowEnery en in table)
            {
                HpSum += (1 - Math.Exp(en.HN));
            }

            foreach (UplinkFlowEnery en in table)
            {
                en.HP = (1 - Math.Exp(en.HN)) / HpSum;
            }
            return table;
        }

        public static List<UplinkFlowEnery> TestRvalue(int neiCount, int disPiovot) 
        {
            List<UplinkFlowEnery> table = new List<UplinkFlowEnery>();
            // normalized values.
            double RSum = 0;
            for (int i = 1; i <= neiCount; i++)
            {
                RSum += disPiovot * i;
            }

            for (int i = 1; i <= neiCount; i++)
            {
                UplinkFlowEnery en = new UplinkFlowEnery();

                en.R = disPiovot * i;
                en.RN = 1 - (Math.Pow(en.R, 1 + Settings.Default.ExpoRCnt) / RSum);
                table.Add(en);
            }

            // pro sum
            double RpSum = 0; // sum of h value.

            foreach (UplinkFlowEnery en in table)
            {
                RpSum += Math.Exp(en.RN);
            }

            foreach (UplinkFlowEnery en in table)
            {
                en.RP = Math.Exp(en.RN) / RpSum;
            }
            return table;
        }


        public static List<UplinkFlowEnery> TestLvalue(int neiCount, int disPiovot) 
        {
            List<UplinkFlowEnery> table = new List<UplinkFlowEnery>();
            // normalized values.
            


            for (int i = 1; i <= neiCount; i++)
            {
                UplinkFlowEnery en = new UplinkFlowEnery();
                en.L = i * disPiovot;
                en.LN = Math.Pow(en.L / 100, Settings.Default.ExpoLCnt);
                table.Add(en);
            }

            // pro sum
            double LpSum = 0;

            foreach (UplinkFlowEnery en in table)
            {
                LpSum += (1 - Math.Exp(-en.LN));
            }

            foreach (UplinkFlowEnery en in table)
            {
                en.LP = (1 - Math.Exp(-en.LN)) / LpSum;
            }
            return table;
        }

        /// <summary>
        /// 5, 200, 10
        /// </summary>
        /// <param name="neiCount"></param>
        /// <param name="disPiovot"></param>
        /// <returns></returns>
        public static List<DownlinkFlowEnery> TestDvalue(int neiCount,int step, int disPiovot) 
        {
            List<DownlinkFlowEnery> table = new List<DownlinkFlowEnery>();
            // normalized values.

            for (int i = 1; i <= neiCount; i++)
            {
                DownlinkFlowEnery en = new DownlinkFlowEnery();
                en.D = step + (disPiovot * i);
                en.DN = (en.D) / ((step + (disPiovot * (neiCount + 1))));
                table.Add(en);
            }

            // pro sum
            double DpSum = 0;

            foreach (DownlinkFlowEnery en in table)
            {
                DpSum += (Math.Pow((1 - Math.Sqrt(en.DN)), 1 + Settings.Default.ExpoDCnt));
            }

            foreach (DownlinkFlowEnery en in table)
            {
                en.DP = (Math.Pow((1 - Math.Sqrt(en.DN)), 1 + Settings.Default.ExpoDCnt)) / DpSum;
            }
            return table;
        }

        public static List<UplinkFlowEnery> TestPvalue(int neiCount, int H_piovot,int R_piovot, int L_piovot)
        {

            List<UplinkFlowEnery> H = TestHvalue(neiCount, H_piovot);
            List<UplinkFlowEnery> R = TestRvalue(neiCount, R_piovot);
            List<UplinkFlowEnery> L = TestLvalue(neiCount, L_piovot);

            double sum = 0;
            for (int i = 0; i < H.Count; i++)
            {
                H[i].L = L[i].L;
                H[i].LN = L[i].LN;
                H[i].LP = L[i].LP;

                H[i].R = R[i].R;
                H[i].RN = R[i].RN;
                H[i].RP = R[i].RP;


                sum += H[i].Mul;

            }


            foreach (UplinkFlowEnery en in H)
            {
                en.Pr = (en.Mul / sum);
            }



            return H;

        }


        }
}
