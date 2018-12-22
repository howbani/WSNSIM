using MiniSDN.ExpermentsResults;
using MiniSDN.Dataplane;
using MiniSDN.ui.conts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MiniSDN.Intilization;
using MiniSDN.Forwarding;

namespace MiniSDN.ui
{
    /// <summary>
    /// Interaction logic for UiSelectNodesWidthDistance.xaml
    /// </summary>
    public partial class UiSelectNodesWidthDistance : Window
    {
        MainWindow mianWind;
        public UiSelectNodesWidthDistance(MainWindow _mianWind)
        {
            InitializeComponent();
            mianWind = _mianWind;

            for (int i = 50; i <= 250; i++)
            {
                com_distance.Items.Add(i);
            }

            for (int i = 1; i < mianWind.myNetWork.Count; i++)
            {
                com_nos.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                com_nop.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }
        }


        private List<Sensor> SelectNodesWithinDistance(double distance)
        {
            List<Sensor> NodesWidDistance = new List<Sensor>();
            if (distance > (PublicParamerters.SensingRangeRadius * 1.2))
            {
                double maxDi = distance + (PublicParamerters.SensingRangeRadius / 2);
                double MinDi = distance - (PublicParamerters.SensingRangeRadius / 2);

                foreach (Sensor s in mianWind.myNetWork)
                {
                    double senDis = Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, s);
                    if (senDis >= MinDi && senDis <= maxDi)
                    {
                        NodesWidDistance.Add(s);
                    }
                }
                return NodesWidDistance;
            }
            else
            {

                return null;
            }
        }

        private void btn_compute_Click(object sender, RoutedEventArgs e)
        {
            PublicParamerters.FinishedRoutedPackets.Clear();
           
            int NOS = Convert.ToInt16(com_nos.Text);
            int NOP = Convert.ToInt16(com_nop.Text);
            double dist = Convert.ToDouble(com_distance.Text);

            List<Sensor> SelectNodesWithinDistanceN = SelectNodesWithinDistance(dist);


            if (SelectNodesWithinDistanceN != null)
            {
                // selecte The Nodes:
                List<Sensor> SelectedSn = new List<Sensor>(NOS);
                for (int i = 0; i < NOS; i++)
                {
                    int ran = Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(SelectNodesWithinDistanceN.Count - 1));
                    if (ran > 0)
                    {
                        SelectedSn.Add(SelectNodesWithinDistanceN[ran]);
                    }
                    else
                    {
                        SelectedSn.Add(SelectNodesWithinDistanceN[1]);
                    }
                }

                foreach (Sensor sen in SelectedSn)
                {

                    sen.GenerateMultipleDataPackets(NOP);
                }
            
            }
        }
    }
}






            
    

