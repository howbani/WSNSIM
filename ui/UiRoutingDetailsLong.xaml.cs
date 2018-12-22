using MiniSDN.Dataplane;
using System.Collections.Generic;
using System.Windows;

namespace MiniSDN.ui
{
    /// <summary>
    /// Interaction logic for UiRoutingDetailsLong.xaml
    /// </summary>
    public partial class UiRoutingDetailsLong : Window
    {
        public UiRoutingDetailsLong( List<Sensor> Network)
        {
            InitializeComponent();

            List<RoutingLog> Logs = new List<RoutingLog>(); 
            foreach(Sensor sen in Network)
            {
                Logs.AddRange(sen.Logs);
            }

            dg_routingLogs.ItemsSource = Logs;

           
        }
    }
}
