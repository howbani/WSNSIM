using MiniSDN.Dataplane;
using MiniSDN.Dataplane.NOS;
using MiniSDN.Properties;
using MiniSDN.ui;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace MiniSDN.ExpermentsResults.Energy_consumptions
{
    class ResultsObject
    {
        public double AverageEnergyConsumption { get; set; }
        public double AverageHops { get; set; }
        public double AverageWaitingTime { get; set; }
        public double AverageRedundantTransmissions { get; set; }
        public double AverageRoutingDistance { get; set; }
        public double AverageTransmissionDistance { get; set; }
    }

    public class ValParPair
    {
        public string Par { get; set; }
        public string Val { get; set; }
    }

    /// <summary>
    /// Interaction logic for ExpReport.xaml
    /// </summary>
    public partial class ExpReport : Window
    {

        public ExpReport(MainWindow _mianWind)
        {
            InitializeComponent();

            List<ValParPair> List = new List<ValParPair>();
            ResultsObject res = new ResultsObject();
            

            double hopsCoun = 0;
            double routingDisEf = 0;
            double avergTransDist = 0;
            foreach (Packet pk in PublicParamerters.FinishedRoutedPackets)
            {
                hopsCoun += pk.Hops;
                routingDisEf += pk.RoutingDistanceEfficiency;
                avergTransDist += pk.AverageTransDistrancePerHop;
            }

            double NumberOfGenPackets = Convert.ToDouble(PublicParamerters.NumberofGeneratedPackets);
            double NumberofDeliveredPacket = Convert.ToDouble(PublicParamerters.NumberofDeliveredPacket);
            double succesRatio = PublicParamerters.DeliveredRatio;

            res.AverageEnergyConsumption = PublicParamerters.TotalEnergyConsumptionJoule;
            double averageWaitingTime = Convert.ToDouble(PublicParamerters.TotalWaitingTime) / NumberofDeliveredPacket;
            res.AverageWaitingTime = averageWaitingTime;
            double avergaeRedundan = Convert.ToDouble(PublicParamerters.TotalReduntantTransmission) / NumberofDeliveredPacket;
            res.AverageRedundantTransmissions = avergaeRedundan;
            res.AverageHops = hopsCoun / NumberofDeliveredPacket;
            res.AverageRoutingDistance = routingDisEf / NumberofDeliveredPacket;
            res.AverageTransmissionDistance = avergTransDist / NumberofDeliveredPacket;

            List.Add(new ValParPair() {Par="Number of Nodes", Val= _mianWind.myNetWork.Count.ToString() } );
            List.Add(new ValParPair() { Par = "Communication Range Radius", Val = PublicParamerters.CommunicationRangeRadius.ToString()+" m"});
            List.Add(new ValParPair() { Par = "Density", Val = PublicParamerters.Density.ToString()});
            List.Add(new ValParPair() { Par = "Packet Rate", Val = _mianWind.PacketRate });
            List.Add(new ValParPair() { Par = "Simulation Time", Val = _mianWind.stopSimlationWhen.ToString()+" s" });
            List.Add(new ValParPair() { Par = "Start up time", Val = PublicParamerters.MacStartUp.ToString() + " s" });
            List.Add(new ValParPair() { Par = "Active Time", Val = PublicParamerters.Periods.ActivePeriod.ToString() + " s" });
            List.Add(new ValParPair() { Par = "Sleep Time", Val = PublicParamerters.Periods.SleepPeriod.ToString() + " s" });
            List.Add(new ValParPair() { Par = "Initial Energy (J)", Val = PublicParamerters.BatteryIntialEnergy.ToString() });
            List.Add(new ValParPair() { Par = "Queue Time", Val = PublicParamerters.QueueTime.Seconds.ToString() });


            List.Add(new ValParPair() { Par = "Total Energy Consumption (J)", Val = res.AverageEnergyConsumption.ToString() });
           
            List.Add(new ValParPair() { Par = "Average Hops/path", Val = res.AverageHops.ToString() });
            List.Add(new ValParPair() { Par = "Average Redundant Transmissions/path", Val = res.AverageRedundantTransmissions.ToString() });
            List.Add(new ValParPair() { Par = "Average Routing Distance/path", Val = res.AverageRoutingDistance.ToString() });
            List.Add(new ValParPair() { Par = "Average Transmission Distance/Hop", Val = res.AverageTransmissionDistance.ToString() });
            List.Add(new ValParPair() { Par = "Average Waiting Time/path", Val = res.AverageWaitingTime.ToString() });


            List.Add(new ValParPair() { Par = "# gen pck", Val = NumberOfGenPackets.ToString() });
            List.Add(new ValParPair() { Par = "# del pck", Val = NumberofDeliveredPacket.ToString() });
            List.Add(new ValParPair() { Par = "# droped pck", Val = PublicParamerters.NumberofDropedPacket.ToString() });
            List.Add(new ValParPair() { Par = "Success %", Val = succesRatio.ToString() });
            List.Add(new ValParPair() { Par = "Droped %", Val = PublicParamerters.DropedRatio.ToString() });

            List.Add(new ValParPair() { Par = "Control Energy Consumption (J)", Val = PublicParamerters.EnergyComsumedForControlPackets.ToString() });
            List.Add(new ValParPair() { Par = "Control Energy Consumption Percentage(%)", Val = PublicParamerters.ControlPacketsEnergyConsmPercentage.ToString() });
            List.Add(new ValParPair() { Par = "# cont pck", Val = PublicParamerters.NumberofControlPackets.ToString() });
            List.Add(new ValParPair() { Par = "cont pck %", Val = PublicParamerters.ControlPacketsPercentage.ToString() });


            List.Add(new ValParPair() { Par = "Update Energy", Val = PublicParamerters.UpdateLossPercentage.ToString() });
            List.Add(new ValParPair() { Par = "L-Control", Val = Settings.Default.ExpoLCnt.ToString() });
            List.Add(new ValParPair() { Par = "H-Control", Val = Settings.Default.ExpoHCnt.ToString() });
            List.Add(new ValParPair() { Par = "R-Control", Val = Settings.Default.ExpoRCnt.ToString() });
            List.Add(new ValParPair() { Par = "Dir-Control", Val = Settings.Default.ExpoECnt.ToString() });
            List.Add(new ValParPair() { Par = "D-Control", Val = Settings.Default.ExpoDCnt.ToString() });

            List.Add(new ValParPair() { Par = "Protocol", Val = "Mini-Flow" });
            dg_data.ItemsSource = List;
        }
    }
}
