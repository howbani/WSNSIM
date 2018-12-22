using MiniSDN.Forwarding;
using MiniSDN.Dataplane;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MiniSDN.Dataplane.PacketRouter;

namespace MiniSDN.ui
{
    public class SensoreBasicsDetails
    {
        public string NodeID { get; set; }
        public string RealLocation { get; set; } // real location
        public string CenterLocation { get; set; } // center location
        public string NeighborsNodes { get; set; }
        public int NeighborsCount { get; set; }
        public string ForwardersStrings { get; set; }
        public int ForwardersCount { get; set; }
    }

    public partial class UIshowSensorsLocations : Window
    {

        private string FindOverlappingNodesString(Sensor node)
        {
            string str = "";
            if (node.NeighborsTable != null)
            {
                foreach (NeighborsTableEntry _node in node.NeighborsTable)
                {
                    str += _node.ID.ToString() + ",";
                }
            }
            return str;
        }

        private string FindForwarders(Sensor node)
        {
           
            return "";
        }


        public UIshowSensorsLocations(List<Sensor> SensorsNodes)
        {
            InitializeComponent();

            List<SensoreBasicsDetails> NodesLocationsList = new List<SensoreBasicsDetails>();
            foreach (Sensor node in SensorsNodes)
            {

                if (node.NeighborsTable != null)
                {
                    SensoreBasicsDetails Sensorinfo = new SensoreBasicsDetails();
                    Sensorinfo.NodeID = node.lbl_Sensing_ID.Content.ToString();
                    Sensorinfo.RealLocation = node.Position.X + "," + node.Position.Y;
                    Sensorinfo.CenterLocation = (node.Position.X + node.ComunicationRangeRadius) + "," + (node.Position.Y + node.ComunicationRangeRadius);
                    Sensorinfo.NeighborsNodes = FindOverlappingNodesString(node);
                    Sensorinfo.ForwardersStrings = FindForwarders(node);
                    Sensorinfo.NeighborsCount = node.NeighborsTable.Count;
                    NodesLocationsList.Add(Sensorinfo);
                }
            }
            dg_locations.ItemsSource = NodesLocationsList;
        }
    }
}

