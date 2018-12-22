using MiniSDN.Dataplane;
using MiniSDN.Properties;
using MiniSDN.ui;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MiniSDN.db
{
    /// <summary>
    /// Interaction logic for NetwokImport.xaml
    /// </summary>
    public partial class NetwokImport : UserControl
    {
        public MainWindow MainWindow { set; get; }
        public List<ImportedSensor> ImportedSensorSensors = new List<ImportedSensor>();

        public UiImportTopology UiImportTopology { get; set; }
        public NetwokImport()
        {
            InitializeComponent();
        }

        private void brn_import_Click(object sender, RoutedEventArgs e)
        {
            NetworkTopolgy.ImportNetwok(this);
            PublicParamerters.NetworkName = lbl_network_name.Content.ToString();
            PublicParamerters.SensingRangeRadius = ImportedSensorSensors[0].R;
            // now add them to feild.

            foreach (ImportedSensor imsensor in ImportedSensorSensors)
            {
                Sensor node = new Sensor(imsensor.NodeID);
                node.MainWindow = MainWindow;
                Point p = new Point(imsensor.Pox, imsensor.Poy);
                node.Position = p;
                node.VisualizedRadius = imsensor.R;
                MainWindow.myNetWork.Add(node);
                MainWindow.Canvas_SensingFeild.Children.Add(node);


                node.ShowID(Settings.Default.ShowID);
                node.ShowSensingRange(Settings.Default.ShowSensingRange);
                node.ShowComunicationRange(Settings.Default.ShowComunicationRange);
                node.ShowBattery(Settings.Default.ShowBattry);
            }
           

            try
            {
                UiImportTopology.Close();
            }
            catch
            {

            }
            

           

        }
    }
}
