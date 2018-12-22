using MiniSDN.Dataplane;
using MiniSDN.Dataplane;
using MiniSDN.Forwarding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MiniSDN.Intilization;

namespace MiniSDN.ui
{
    /// <summary>
    /// Interaction logic for UiAddNodes.xaml
    /// </summary>
    public partial class UiAddNodes : Window
    {
       
        public MainWindow MainWindow { get; set; }
        public UiAddNodes()
        {
            InitializeComponent();
        }


        private static double RdmGenerator(double max)
        {
            return max* RandomeNumberGenerator.GetUniform();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int nodeCount = MainWindow.Canvas_SensingFeild.Children.Count;
                double r= Convert.ToDouble(txt_range.Text);
                PublicParamerters.SensingRangeRadius = r;
                if (txt_nodes_number.Text.Trim().Length > 0)
                {
                    int NodesNumber = Convert.ToInt16(txt_nodes_number.Text);
#pragma warning disable CS0219 // The variable 'rd' is assigned but its value is never used
                    double rd = 1;
#pragma warning restore CS0219 // The variable 'rd' is assigned but its value is never used
                    Random rnd = new Random();
                    double x = 0;
                    double y = 0;
                    for (int id = 0; id <NodesNumber; id++)
                    {

                        int ID = nodeCount + id;
                        Sensor node = new Sensor(ID);
                        node.MainWindow = MainWindow;

                        x = RdmGenerator(MainWindow.Canvas_SensingFeild.Width - 50);
                        //  Thread.Sleep(TimeSpan.FromMilliseconds(1));
                        y = RdmGenerator(MainWindow.Canvas_SensingFeild.Height - 50);
                      //  Thread.Sleep(TimeSpan.FromMilliseconds(1));


                        Point p = new Point(x, y);
                        node.Position = p;
                        node.VisualizedRadius = r;
                        MainWindow.Canvas_SensingFeild.Children.Add(node);
                        MainWindow.myNetWork.Add(node);

                        
                    }
                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            finally
            {
                this.Close();
            }

          
        }
    }
}
