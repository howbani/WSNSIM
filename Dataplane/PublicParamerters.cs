using MiniSDN.Dataplane;
using System;
using System.Collections.Generic;
using MiniSDN.Energy;
using MiniSDN.ExpermentsResults.Lifetime;
using MiniSDN.ui;
using System.Windows.Media;
using MiniSDN.ControlPlane.NOS;
using MiniSDN.Dataplane.NOS;
using MiniSDN.Properties;

namespace MiniSDN.Dataplane
{
    /// <summary>
    /// 
    /// </summary>
    public class PublicParamerters
    {
        public static long NumberofControlPackets { get; set; }
        public static double EnergyComsumedForControlPackets { get; set; }
         

        public static long NumberofDropedPacket { get; set; } 
        public static long NumberofDeliveredPacket { get; set; } // the number of the pakctes recived in the sink node.
        public static long Rounds { get; set; } // how many rounds.
        public static List<DeadNodesRecord> DeadNodeList = new List<DeadNodesRecord>();
        public static long NumberofGeneratedPackets { get; set; }
        public static long TotalWaitingTime { get; set; } // how many times the node waitted for its coordinate to wake up.
        public static long TotalReduntantTransmission { get; set; } // how many transmission are redundant, that is to say, recived and canceled.
        public static bool IsNetworkDied { get; set; } // yes if the first node deide.
        public static double SensingRangeRadius { get; set; }
        public static double Density { get; set; } // average number of neighbores (stander deiviation)
        public static string NetworkName { get; set; }
        public static Sensor SinkNode { get; set; }
        public static double BatteryIntialEnergy { get { return Settings.Default.BatteryIntialEnergy; } } //J 0.5 /////////////*******////////////////////////////////////
        public static double BatteryIntialEnergyForSink = 500; //500J.
        public static double RoutingDataLength = 1024; // bit
        public static double ControlDataLength = 512; // bit
        public static double PreamblePacketLength = 128; // bit 
        public static double E_elec = 50; // unit: (nJ/bit) //Energy dissipation to run the radio
        public static double Efs = 0.01;// unit( nJ/bit/m^2 ) //Free space model of transmitter amplifier
        public static double Emp = 0.0000013; // unit( nJ/bit/m^4) //Multi-path model of transmitter amplifier
        public static double CommunicationRangeRadius { get { return SensingRangeRadius * 2; } } // sensing range is R in the DB.
        public static double TransmissionRate = 2 * 1000000;////2Mbps 100 × 10^6 bit/s , //https://en.wikipedia.org/wiki/Transmission_time
        public static double SpeedOfLight = 299792458;//https://en.wikipedia.org/wiki/Speed_of_light // s
        public static string PowersString { get; set; }
        public static double TotalEnergyConsumptionJoule { get; set; } // keep all energy consumption. 
        public static double TotalWastedEnergyJoule { get; set; } // idel listening energy
        public static double TotalDelayMs { get; set; } // in ms 
        public static List<Packet> FinishedRoutedPackets = new List<Packet>(); // all the packets whatever dliverd or not.
        public static double ThresholdDistance  //Distance threshold ( unit m) 
        {
            get { return Math.Sqrt(Efs / Emp); }
        }


        public static double ControlPacketsPercentage { get { return 100 * (NumberofControlPackets / NumberofGeneratedPackets); } }
        public static double ControlPacketsEnergyConsmPercentage { get { return 100 * (EnergyComsumedForControlPackets / TotalEnergyConsumptionJoule); } } 




        public static double WastedEnergyPercentage { get { return 100 * (TotalWastedEnergyJoule / TotalEnergyConsumptionJoule); } } // idel listening energy percentage  

        public static List<Color> RandomColors { get; set; }

        public static double SensingFeildArea
        {
            get; set;
        }
        public static int NumberofNodes
        {
            get; set;
        }

        public static long InQueuePackets
        {
            get
            {
                return NumberofGeneratedPackets - NumberofDeliveredPacket - NumberofDropedPacket;
            }
        }

        public static double DeliveredRatio
        {
            get
            {
                return 100 * (Convert.ToDouble(NumberofDeliveredPacket) / Convert.ToDouble(NumberofGeneratedPackets));
            }
        }
        public static double DropedRatio
        {
            get
            {
                return 100 * (Convert.ToDouble(NumberofDropedPacket) / Convert.ToDouble(NumberofGeneratedPackets));
            }
        }

        public static MainWindow MainWindow { get; set; } 

        /// <summary>
        /// Each time when the node loses 5% of its energy, it shares new energy percentage with its neighbors. The neighbor nodes update their energy distributions according to the new percentage immediately as explained by Algorithm 2. 
        /// </summary>
        public static int UpdateLossPercentage
        {
            get
            {
                return Settings.Default.UpdateLossPercentage;
            }
            set
            {
                Settings.Default.UpdateLossPercentage = value;
            }
        }

        // lifetime paramerts:
        public static int NOS { get; set; } // NUMBER OF RANDOM SELECTED SOURCES
        public static int NOP { get; set; } // NUMBER OF PACKETS TO BE SEND.




        /// <summary>
        /// in sec.
        /// </summary>
        public static class Periods
        {
            public static double ActivePeriod { get { return Settings.Default.ActivePeriod; } } //  the node trun on and check for CheckPeriod seconds.// +1
            public static double SleepPeriod { get { return Settings.Default.SleepPeriod; } }  // the node trun off and sleep for SleepPeriod seconds.
        }



        /// <summary>
        /// When all forwarders are sleep, 
        /// the sender try agian until its formwarder is wake up. the sender try agian each 500 ms.
        /// when the sensor retry to send the back is it's forwarders are in sleep mode.
        /// </summary>
        public static TimeSpan QueueTime
        {
            get
            {
                return TimeSpan.FromSeconds(Settings.Default.QueueTime);
            }
        }

        /// <summary>
        /// the timer interval between 1 and 5 sec.
        /// </summary>
        public static int MacStartUp
        {
            get
            {
                return Settings.Default.MacStartUp;
            }
        }

        /// <summary>
        /// the runnunin time of simulator. in SEC
        /// </summary>
        public static int SimulationTime
        {
            get;set;
        }


        public static List<BatRange> getRanges()
        {
            List<BatRange> re = new List<BatRange>();

            int x = 100 / UpdateLossPercentage;
            for (int i = 1; i <= x; i++)
            {
                BatRange r = new Energy.BatRange();
                r.isUpdated = false;
                r.Rang[0] = (i - 1) * UpdateLossPercentage;
                r.Rang[1] = i * UpdateLossPercentage;
                r.ID = i;
                re.Add(r);
            }

            re[re.Count - 1].isUpdated = true;

            return re;
        }
    }
}
