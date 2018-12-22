using MiniSDN.Dataplane;
using System.Collections.Generic;
using System.Windows;

namespace MiniSDN.db
{
    /// <summary>
    /// Interaction logic for UiExportTopology.xaml
    /// </summary>
    public partial class UiExportTopology : Window
    {
        List<Sensor> myNetWork;
        public UiExportTopology(List<Sensor> _myNetWork) 
        {
            InitializeComponent();
            myNetWork = _myNetWork;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           NetworkTopolgy topolog = new NetworkTopolgy();
           bool isExisted= topolog.createNewTopology(txt_networkName.Text);
           if (!isExisted)
           {
               this.WindowState = WindowState.Minimized;
               foreach (Sensor sensor in myNetWork)
               {
                   topolog.SaveSensor(sensor, txt_networkName.Text);
               }
               this.Close();
           }
           else
           {
               MessageBox.Show("please change network name!");
           }
        }
    }
}
