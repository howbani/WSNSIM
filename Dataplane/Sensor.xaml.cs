using MiniSDN.Intilization;
using MiniSDN.Energy;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MiniSDN.ui;
using MiniSDN.Properties;
using System.Windows.Threading;
using System.Threading;
using MiniSDN.ControlPlane.NOS;
using MiniSDN.ui.conts;
using MiniSDN.ControlPlane.NOS.FlowEngin;
using MiniSDN.Forwarding;
using MiniSDN.Dataplane.PacketRouter;
using MiniSDN.Dataplane.NOS;

namespace MiniSDN.Dataplane
{
    public enum SensorState { intalized, Active, Sleep } // defualt is not used. i 
    public enum EnergyConsumption { Transmit, Recive } // defualt is not used. i 


    /// <summary>
    /// Interaction logic for Node.xaml
    /// </summary>
    public partial class Sensor : UserControl
    {
        #region Commone parameters.

        public Radar Myradar; 
        public List<Arrow> MyArrows = new List<Arrow>();
        public MainWindow MainWindow { get; set; } // the mian window where the sensor deployed.
        public static double SR { get; set; } // the radios of SENSING range.
        public double SensingRangeRadius { get { return SR; } }
        public static double CR { get; set; }  // the radios of COMUNICATION range. double OF SENSING RANGE
        public double ComunicationRangeRadius { get { return CR; } }
        public double BatteryIntialEnergy; // jouls // value will not be changed
        private double _ResidualEnergy; //// jouls this value will be changed according to useage of battery
        public List<int> DutyCycleString = new List<int>(); // return the first letter of each state.
        public BoXMAC Mac { get; set; } // the mac protocol for the node.
        public SensorState CurrentSensorState { get; set; } // state of node.
        public List<RoutingLog> Logs = new List<RoutingLog>();
        public List<NeighborsTableEntry> NeighborsTable = null; // neighboring table.
        public List<MiniFlowTableEntry> MiniFlowTable = new List<MiniFlowTableEntry>(); //flow table.
        public int NumberofPacketsGeneratedByMe = 0; // the number of packets sent by this packet.
        public FirstOrderRadioModel EnergyModel = new FirstOrderRadioModel(); // energy model.
        public int ID { get; set; } // the ID of sensor.
        public int HopsToSink = int.MaxValue; // number of hops from the node to the sink.
        public bool trun { get; set; }// this will be true if the node is already sent the beacon packet for discovering the number of hops to the sink.
        private DispatcherTimer SendPacketTimer = new DispatcherTimer();// 
        private DispatcherTimer QueuTimer = new DispatcherTimer();// to check the packets in the queue right now.
        public Queue<Packet> WaitingPacketsQueue = new Queue<Packet>(); // packets queue.
        public List<BatRange> BatRangesList = new List<Energy.BatRange>();

        /// <summary>
        /// CONFROM FROM NANO NO JOUL
        /// </summary>
        /// <param name="UsedEnergy_Nanojoule"></param>
        /// <returns></returns>
        public double ConvertToJoule(double UsedEnergy_Nanojoule) //the energy used for current operation
        {
            double _e9 = 1000000000; // 1*e^-9
            double _ONE = 1;
            double oNE_DIVIDE_e9 = _ONE / _e9;
            double re = UsedEnergy_Nanojoule * oNE_DIVIDE_e9;
            return re;
        }

        /// <summary>
        /// in JOULE
        /// </summary>
        public double ResidualEnergy // jouls this value will be changed according to useage of battery
        {
            get { return _ResidualEnergy; }
            set
            {
                _ResidualEnergy = value;
                Prog_batteryCapacityNotation.Value = _ResidualEnergy;
            }
        } //@unit(JOULS);


        /// <summary>
        /// 0%-100%
        /// </summary>
        public double ResidualEnergyPercentage
        {
            get { return (ResidualEnergy / BatteryIntialEnergy) * 100; }
        }
        /// <summary>
        /// visualized sensing range and comuinication range
        /// </summary>
        public double VisualizedRadius
        {
            get { return Ellipse_Sensing_range.Width / 2; }
            set
            {
                // sensing range:
                Ellipse_Sensing_range.Height = value * 2; // heigh= sen rad*2;
                Ellipse_Sensing_range.Width = value * 2; // Width= sen rad*2;
                SR = VisualizedRadius;
                CR = SR * 2; // comunication rad= sensing rad *2;

                // device:
                Device_Sensor.Width = value * 4; // device = sen rad*4;
                Device_Sensor.Height = value * 4;
                // communication range
                Ellipse_Communication_range.Height = value * 4; // com rang= sen rad *4;
                Ellipse_Communication_range.Width = value * 4;

                // battery:
                Prog_batteryCapacityNotation.Width = 8;
                Prog_batteryCapacityNotation.Height = 2;
            }
        }

        /// <summary>
        /// Real postion of object.
        /// </summary>
        public Point Position
        {
            get
            {
                double x = Device_Sensor.Margin.Left;
                double y = Device_Sensor.Margin.Top;
                Point p = new Point(x, y);
                return p;
            }
            set
            {
                Point p = value;
                Device_Sensor.Margin = new Thickness(p.X, p.Y, 0, 0);
            }
        }

        /// <summary>
        /// center location of node.
        /// </summary>
        public Point CenterLocation
        {
            get
            {
                double x = Device_Sensor.Margin.Left;
                double y = Device_Sensor.Margin.Top;
                Point p = new Point(x + CR, y + CR);
                return p;
            }
        }

        bool StartMove = false; // mouse start move.
        private void Device_Sensor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Settings.Default.IsIntialized == false)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                    P.X = P.X - CR;
                    P.Y = P.Y - CR;
                    Position = P;
                    StartMove = true;
                }
            }
        }

        private void Device_Sensor_MouseMove(object sender, MouseEventArgs e)
        {
            if (Settings.Default.IsIntialized == false)
            {
                if (StartMove)
                {
                    System.Windows.Point P = e.GetPosition(MainWindow.Canvas_SensingFeild);
                    P.X = P.X - CR;
                    P.Y = P.Y - CR;
                    this.Position = P;
                }
            }
        }

        private void Device_Sensor_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StartMove = false;
        }




        private void Prog_batteryCapacityNotation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            double val = ResidualEnergyPercentage;
            if (val <= 0)
            {

                // dead certificate:
                ExpermentsResults.Lifetime.DeadNodesRecord recod = new ExpermentsResults.Lifetime.DeadNodesRecord();
                recod.DeadAfterPackets = PublicParamerters.NumberofGeneratedPackets;
                recod.DeadOrder = PublicParamerters.DeadNodeList.Count + 1;
                recod.Rounds = PublicParamerters.Rounds + 1;
                recod.DeadNodeID = ID;
                recod.NOS = PublicParamerters.NOS;
                recod.NOP = PublicParamerters.NOP;
                PublicParamerters.DeadNodeList.Add(recod);

                Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col0));
                Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col0));


                if (Settings.Default.StopeWhenFirstNodeDeid)
                {
                    MainWindow.TimerCounter.Stop();
                    MainWindow.RandomSelectSourceNodesTimer.Stop();
                    MainWindow.stopSimlationWhen = PublicParamerters.SimulationTime;
                    MainWindow.top_menu.IsEnabled = true;
                }


            }
            if (val >= 1 && val <= 9)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col1_9)));
               Dispatcher.Invoke(()=> Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col1_9)));
            }

            if (val >= 10 && val <= 19)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col10_19)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col10_19)));
            }

            if (val >= 20 && val <= 29)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col20_29)));
                Dispatcher.Invoke(() => Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col20_29))));
            }

            // full:
            if (val >= 30 && val <= 39)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col30_39)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col30_39)));
            }
            // full:
            if (val >= 40 && val <= 49)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col40_49)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col40_49)));
            }
            // full:
            if (val >= 50 && val <= 59)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col50_59)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col50_59)));
            }
            // full:
            if (val >= 60 && val <= 69)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col60_69)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col60_69)));
            }
            // full:
            if (val >= 70 && val <= 79)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col70_79)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col70_79)));
            }
            // full:
            if (val >= 80 && val <= 89)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col80_89)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col80_89)));
            }
            // full:
            if (val >= 90 && val <= 100)
            {
                Dispatcher.Invoke(() => Prog_batteryCapacityNotation.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col90_100)));
                Dispatcher.Invoke(() => Ellipse_battryIndicator.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(BatteryLevelColoring.col90_100)));
            }



            // update the battery distrubtion.
            int battper = Convert.ToInt16(val);
            if (battper > PublicParamerters.UpdateLossPercentage)
            {
                int rangeIndex = battper / PublicParamerters.UpdateLossPercentage;
                if (rangeIndex >= 1)
                {
                    if (BatRangesList.Count > 0)
                    {
                        BatRange range = BatRangesList[rangeIndex - 1];
                        if (battper >= range.Rang[0] && battper <= range.Rang[1])
                        {
                            if (range.isUpdated == false)
                            {
                                range.isUpdated = true;
                                // update the uplink.
                                UplinkRouting.UpdateUplinkFlowEnery(this);

                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// show or hide the arrow in seperated thread.
        /// </summary>
        /// <param name="id"></param>
        public void ShowOrHideArrow(int id) 
        {
            Thread thread = new Thread(() =>
            {
                lock (MyArrows)
                {
                    Arrow ar = GetArrow(id);
                    if (ar != null)
                    {
                        lock (ar)
                        {
                            if (ar.Visibility == Visibility.Visible)
                            {
                                Action action = () => ar.Visibility = Visibility.Hidden;
                                Dispatcher.Invoke(action);
                            }
                            else
                            {
                                Action action = () => ar.Visibility = Visibility.Visible;
                                Dispatcher.Invoke(action);
                            }
                        }
                    }
                }
            }
            );
            thread.Start();
        }


        // get arrow by ID.
        private Arrow GetArrow(int EndPointID)
        {
            foreach (Arrow arr in MyArrows) { if (arr.To.ID == EndPointID) return arr; }
            return null;
        }



       

        #endregion



       
       

        /// <summary>
        /// 
        /// </summary>
        public void SwichToActive()
        {
            Mac.SwichToActive();

        }

        /// <summary>
        /// 
        /// </summary>
        private void SwichToSleep()
        {
            Mac.SwichToSleep();
        }

       
        public Sensor(int nodeID)
        {
            InitializeComponent();
            //: sink is diffrent:
            if (nodeID == 0) BatteryIntialEnergy = PublicParamerters.BatteryIntialEnergyForSink; // the value will not be change
            else
                BatteryIntialEnergy = PublicParamerters.BatteryIntialEnergy;
           
            
            ResidualEnergy = BatteryIntialEnergy;// joules. intializing.
            Prog_batteryCapacityNotation.Value = BatteryIntialEnergy;
            Prog_batteryCapacityNotation.Maximum = BatteryIntialEnergy;
            lbl_Sensing_ID.Content = nodeID;
            ID = nodeID;
            QueuTimer.Interval = PublicParamerters.QueueTime;
            QueuTimer.Tick += DeliveerPacketsInQueuTimer_Tick;
            //:

            SendPacketTimer.Interval = TimeSpan.FromSeconds(1);
           

        }

       

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            

        }

        /// <summary>
        /// hide all arrows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            /*
            Vertex ver = MainWindow.MyGraph[ID];
            foreach(Vertex v in ver.Candidates)
            {
                MainWindow.myNetWork[v.ID].lbl_Sensing_ID.Background = Brushes.Black;
            }*/
         
        }

        

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

       

        public int ComputeMaxHopsUplink
        {
            get
            {
                double  DIS= Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, this);
                return Convert.ToInt16(Math.Ceiling((Math.Sqrt(PublicParamerters.Density) * (DIS / ComunicationRangeRadius))));
            }
        }

        public int ComputeMaxHopsDownlink(Sensor endNode)
        {
            double DIS = Operations.DistanceBetweenTwoSensors(PublicParamerters.SinkNode, endNode);
            return Convert.ToInt16(Math.Ceiling((Math.Sqrt(PublicParamerters.Density) * (DIS / ComunicationRangeRadius))));
        }

        #region send data: /////////////////////////////////////////////////////////////////////////////


        public void IdentifySourceNode(Sensor source)
        {
            if (Settings.Default.ShowAnimation)
            {
                Action actionx = () => source.Ellipse_indicator.Visibility = Visibility.Visible;
                Dispatcher.Invoke(actionx);

                Action actionxx = () => source.Ellipse_indicator.Fill = Brushes.Yellow;
                Dispatcher.Invoke(actionxx);
            }
        }

        public void UnIdentifySourceNode(Sensor source)
        {
            if (Settings.Default.ShowAnimation)
            {
                Action actionx = () => source.Ellipse_indicator.Visibility = Visibility.Hidden;
                Dispatcher.Invoke(actionx);

                Action actionxx = () => source.Ellipse_indicator.Fill = Brushes.Transparent;
                Dispatcher.Invoke(actionxx);
            }
        }

        /// <summary>
        /// uplink routing packets
        /// </summary>
        public void GenerateDataPacket()
        {
            if (Settings.Default.IsIntialized)
            {
                
                PublicParamerters.NumberofGeneratedPackets += 1;
                Packet packet = new Packet();
                packet.Path = "" + this.ID;
                packet.TimeToLive = this.ComputeMaxHopsUplink;
                packet.Source = this;
                packet.PacketLength = PublicParamerters.RoutingDataLength;
                packet.PacketType = PacketType.Data;
                packet.PID = PublicParamerters.NumberofGeneratedPackets;
                packet.Destination = PublicParamerters.SinkNode;
                IdentifySourceNode(this);
                MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_num_of_gen_packets.Content = PublicParamerters.NumberofGeneratedPackets, DispatcherPriority.Normal);
                //:
                this.SendPacekt(packet);
              
                

            }
        }

        public void GenerateMultipleDataPackets(int numOfPackets)
        {
            for (int i = 0; i < numOfPackets; i++)
            {
                GenerateDataPacket();
              //  Thread.Sleep(50);
            }
        }

       

       

        /// <summary>
        /// downlink
        /// </summary>
        /// <param name="Destination"></param>
        public void GenerateControlPacket(Sensor endNode)
        {
            if (Settings.Default.IsIntialized)
            {

                PublicParamerters.NumberofGeneratedPackets += 1; // all packets.
                PublicParamerters.NumberofControlPackets += 1; // this for control.
                Packet packet = new Packet();
                packet.Path = "" + this.ID;
                packet.TimeToLive = ComputeMaxHopsDownlink(endNode);
                packet.Source = PublicParamerters.SinkNode; // the sink.
                packet.PacketLength = PublicParamerters.ControlDataLength;
                packet.PacketType = PacketType.Control;
                packet.PID = PublicParamerters.NumberofGeneratedPackets;
                packet.Destination = endNode;
                IdentifyEndNode(endNode);

                MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_number_of_control_packets.Content = PublicParamerters.NumberofControlPackets, DispatcherPriority.Normal);
                MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_num_of_gen_packets.Content = PublicParamerters.NumberofGeneratedPackets, DispatcherPriority.Normal);
                this.SendPacekt(packet);

               // Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
        /// <summary>
        /// to the same endnode.
        /// </summary>
        /// <param name="numOfPackets"></param>
        /// <param name="endone"></param>
        public void GenerateMultipleControlPackets(int numOfPackets,Sensor endone)
        {
            for (int i = 0; i < numOfPackets; i++)
            {
                GenerateControlPacket(endone);
            }
        }

        public void IdentifyEndNode(Sensor endNode)
        {
            if (Settings.Default.ShowAnimation)
            {
                Action actionx = () => endNode.Ellipse_indicator.Visibility = Visibility.Visible;
                Dispatcher.Invoke(actionx);

                Action actionxx = () => endNode.Ellipse_indicator.Fill = Brushes.DarkOrange;
                Dispatcher.Invoke(actionxx);
            }
        }

        public void UnIdentifyEndNode(Sensor endNode)
        {
            if (Settings.Default.ShowAnimation)
            {
                Action actionx = () => endNode.Ellipse_indicator.Visibility = Visibility.Hidden;
                Dispatcher.Invoke(actionx);

                Action actionxx = () => endNode.Ellipse_indicator.Fill = Brushes.Transparent;
                Dispatcher.Invoke(actionxx);
            }
        }


        /// <summary>
        ///  select this node as a source and let it 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btn_send_packet_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lbl_title = sender as Label;
            switch (lbl_title.Name)
            {
                case "btn_send_1_packet":
                    {
                        if (this.ID != PublicParamerters.SinkNode.ID)
                        {
                            // uplink:
                            GenerateMultipleDataPackets(1);
                        }
                        else
                        {
                            RandomSelectEndNodes(1);
                        }
                       
                        break;
                    }
                case "btn_send_10_packet":
                    {
                        if (this.ID != PublicParamerters.SinkNode.ID)
                        {
                            // uplink:
                            GenerateMultipleDataPackets(10);
                        }
                        else
                        {
                            RandomSelectEndNodes(10);
                        }
                        break;
                    }

                case "btn_send_100_packet":
                    {
                        if (this.ID != PublicParamerters.SinkNode.ID)
                        {
                            // uplink:
                            GenerateMultipleDataPackets(100);
                        }
                        else
                        {
                            RandomSelectEndNodes(100);
                        }
                        break;
                    }

                case "btn_send_300_packet":
                    {
                        if (this.ID != PublicParamerters.SinkNode.ID)
                        {
                            // uplink:
                            GenerateMultipleDataPackets(300);
                        }
                        else
                        {
                            RandomSelectEndNodes(300);
                        }
                        break;
                    }

                case "btn_send_1000_packet":
                    {
                        if (this.ID != PublicParamerters.SinkNode.ID)
                        {
                            // uplink:
                            GenerateMultipleDataPackets(1000);
                        }
                        else
                        {
                            RandomSelectEndNodes(1000);
                        }
                        break;
                    }

                case "btn_send_5000_packet":
                    {
                        if (this.ID != PublicParamerters.SinkNode.ID)
                        {
                            // uplink:
                            GenerateMultipleDataPackets(5000);
                        }
                        else
                        {
                            // DOWN
                            RandomSelectEndNodes(5000);
                        }
                        break;
                    }
            }
        }

        // try.
        private void DeliveerPacketsInQueuTimer_Tick(object sender, EventArgs e)
        {
           

            Packet toppacket = WaitingPacketsQueue.Dequeue();
            Console.WriteLine("NID:" + this.ID + " trying(preamble packet) to sent The  PID:" + toppacket.PID);
            toppacket.WaitingTimes += 1;
            PublicParamerters.TotalWaitingTime += 1; // total;
            SendPacekt(toppacket);
            if (WaitingPacketsQueue.Count == 0)
            {
                if(Settings.Default.ShowRadar) Myradar.StopRadio();

                QueuTimer.Stop();
                Console.WriteLine("NID:" + this.ID + ". Queu Timer is stoped.");
                MainWindow.Dispatcher.Invoke(() => Ellipse_indicator.Fill = Brushes.Transparent);
                MainWindow.Dispatcher.Invoke(() => Ellipse_indicator.Visibility = Visibility.Hidden);
            }
        }




        public void RedundantTransmisionCost(Packet pacekt, Sensor reciverNode)
        {
            // logs.
            PublicParamerters.TotalReduntantTransmission += 1;
            double UsedEnergy_Nanojoule = EnergyModel.Receive(PublicParamerters.PreamblePacketLength); // preamble packet length.
            double UsedEnergy_joule = ConvertToJoule(UsedEnergy_Nanojoule);
            reciverNode.ResidualEnergy = reciverNode.ResidualEnergy - UsedEnergy_joule;
            pacekt.UsedEnergy_Joule += UsedEnergy_joule;
            PublicParamerters.TotalEnergyConsumptionJoule += UsedEnergy_joule;
            PublicParamerters.TotalWastedEnergyJoule += UsedEnergy_joule;
            MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_Redundant_packets.Content = PublicParamerters.TotalReduntantTransmission);
            MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_Wasted_Energy_percentage.Content = PublicParamerters.WastedEnergyPercentage);
        }

        /// <summary>
        /// the node which is active will send preample packet and will be selected.
        /// match the packet.
        /// </summary>
        public MiniFlowTableEntry MatchFlow(Packet pacekt)
        {
            MiniFlowTableEntry ret = null;
            try
            {
                if (MiniFlowTable.Count > 0)
                {
                    foreach (MiniFlowTableEntry selectedflow in MiniFlowTable)
                    {
                        if (pacekt.PacketType == PacketType.Data && selectedflow.SensorState == SensorState.Active && selectedflow.UpLinkAction == FlowAction.Forward)
                        {
                            if (ret == null) { ret = selectedflow; }
                            else
                            {
                                RedundantTransmisionCost(pacekt, selectedflow.NeighborEntry.NeiNode);
                            }
                        }
                        else if (pacekt.PacketType == PacketType.Control && selectedflow.SensorState == SensorState.Active && selectedflow.DownLinkAction == FlowAction.Forward)
                        {
                            if (ret == null) { ret = selectedflow; }
                            else
                            {
                                // logs.
                                RedundantTransmisionCost(pacekt, selectedflow.NeighborEntry.NeiNode);
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No Flow!!!. muach flow!");
                    return null;
                }
            }
            catch
            {
                ret = null;
              //  MessageBox.Show(" Null Match.!");
            }

            return ret;
        }

        // When the sensor open the channel to transmit the data.
        private void OpenChanel(int reciverID, long PID)
        {
            Thread thread = new Thread(() =>
            {
                lock (MyArrows)
                {
                    Arrow ar = GetArrow(reciverID);
                    if (ar != null)
                    {
                        lock (ar)
                        {
                            if (ar.Visibility == Visibility.Hidden)
                            {
                                if (Settings.Default.ShowAnimation)
                                {
                                    Action actionx = () => ar.BeginAnimation(PID);
                                    Dispatcher.Invoke(actionx);
                                    Action action1 = () => ar.Visibility = Visibility.Visible;
                                    Dispatcher.Invoke(action1);
                                }
                                else
                                {
                                    Action action1 = () => ar.Visibility = Visibility.Visible;
                                    Dispatcher.Invoke(action1);
                                    Dispatcher.Invoke(() => ar.Stroke = new SolidColorBrush(Colors.Black));
                                    Dispatcher.Invoke(() => ar.StrokeThickness = 1);
                                    Dispatcher.Invoke(() => ar.HeadHeight = 1);
                                    Dispatcher.Invoke(() => ar.HeadWidth = 1);
                                }
                            }
                            else
                            {
                                if (Settings.Default.ShowAnimation)
                                {
                                    int cid = Convert.ToInt16(PID % PublicParamerters.RandomColors.Count);
                                    Action actionx = () => ar.BeginAnimation(PID);
                                    Dispatcher.Invoke(actionx);
                                    Dispatcher.Invoke(() => ar.HeadHeight = 1);
                                    Dispatcher.Invoke(() => ar.HeadWidth = 1);
                                }
                                else
                                {
                                    Dispatcher.Invoke(() => ar.Stroke = new SolidColorBrush(Colors.Black));
                                    Dispatcher.Invoke(() => ar.StrokeThickness = 1);
                                    Dispatcher.Invoke(() => ar.HeadHeight = 1);
                                    Dispatcher.Invoke(() => ar.HeadWidth = 1);
                                }
                            }
                        }
                    }
                }
            }
           );
            thread.Start();
            thread.Priority = ThreadPriority.Highest;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Reciver"></param>
        /// <param name="packt"></param>
        public void SendPacekt(Packet packt)
        {
            if (packt.PacketType == PacketType.Data)
            {
                lock (MiniFlowTable)
                {
                    MiniFlowTable.Sort(new MiniFlowTableSorterUpLinkPriority());
                    MiniFlowTableEntry flowEntry = MatchFlow(packt);
                    if (flowEntry != null)
                    {
                        Sensor Reciver = flowEntry.NeighborEntry.NeiNode;
                        // sender swich on the redio:
                      //  SwichToActive();
                        ComputeOverhead(packt, EnergyConsumption.Transmit, Reciver);
                        Console.WriteLine("sucess:" + ID + "->" + Reciver.ID + ". PID: " + packt.PID);
                        flowEntry.UpLinkStatistics += 1;
                       // Reciver.SwichToActive();
                        Reciver.ReceivePacket(packt);
                      //  SwichToSleep();// .
                    }
                    else
                    {
                        // no available node right now.
                        // add the packt to the wait list.
                        Console.WriteLine("NID:" + ID + " Faild to sent PID:" + packt.PID);
                        WaitingPacketsQueue.Enqueue(packt);
                        QueuTimer.Start();
                        Console.WriteLine("NID:" + ID + ". Queu Timer is started.");
                      //  SwichToSleep();// this.
                        if (Settings.Default.ShowRadar) Myradar.StartRadio();
                        PublicParamerters.MainWindow.Dispatcher.Invoke(() => Ellipse_indicator.Fill = Brushes.DeepSkyBlue);
                        PublicParamerters.MainWindow.Dispatcher.Invoke(() => Ellipse_indicator.Visibility = Visibility.Visible);
                    }
                }
            }
            else if (packt.PacketType == PacketType.Control)
            {
                lock (MiniFlowTable)
                {
                    DownLinkRouting.GetD_Distribution(this, packt.Destination);
                    MiniFlowTableEntry FlowEntry = MatchFlow(packt);
                    if (FlowEntry != null)
                    {
                        Sensor Reciver = FlowEntry.NeighborEntry.NeiNode;
                        // sender swich on the redio:
                     //   SwichToActive(); // this.
                        ComputeOverhead(packt, EnergyConsumption.Transmit, Reciver);
                        FlowEntry.DownLinkStatistics += 1;
                       // Reciver.SwichToActive();
                        Reciver.ReceivePacket(packt);
                      //  SwichToSleep();// this.
                    }
                    else
                    {
                        // no available node right now.
                        // add the packt to the wait list.
                        Console.WriteLine("NID:" + this.ID + " Faild to sent PID:" + packt.PID);
                        WaitingPacketsQueue.Enqueue(packt);
                        QueuTimer.Start();
                        Console.WriteLine("NID:" + this.ID + ". Queu Timer is started.");
                        //  SwichToSleep();// sleep.
                        if (Settings.Default.ShowRadar) Myradar.StartRadio();
                        PublicParamerters.MainWindow.Dispatcher.Invoke(() => Ellipse_indicator.Fill = Brushes.DeepSkyBlue);
                        PublicParamerters.MainWindow.Dispatcher.Invoke(() => Ellipse_indicator.Visibility = Visibility.Visible);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packt"></param>
        /// <param name="enCon"></param>
        /// <param name="Reciver"></param>
        public void ComputeOverhead(Packet packt, EnergyConsumption enCon, Sensor Reciver)
        {
            if (enCon == EnergyConsumption.Transmit)
            {
                if (ID != PublicParamerters.SinkNode.ID)
                {
                    // calculate the energy 
                    double Distance_M = Operations.DistanceBetweenTwoSensors(this, Reciver);
                    double UsedEnergy_Nanojoule = EnergyModel.Transmit(packt.PacketLength, Distance_M);
                    double UsedEnergy_joule = ConvertToJoule(UsedEnergy_Nanojoule);
                    ResidualEnergy = this.ResidualEnergy - UsedEnergy_joule;
                    PublicParamerters.TotalEnergyConsumptionJoule += UsedEnergy_joule;
                    packt.UsedEnergy_Joule += UsedEnergy_joule;
                    packt.RoutingDistance += Distance_M;
                    packt.Hops += 1;
                    double delay = DelayModel.DelayModel.Delay(this, Reciver);
                    packt.Delay += delay;
                    PublicParamerters.TotalDelayMs += delay;
                    if (Settings.Default.SaveRoutingLog)
                    {
                        RoutingLog log = new RoutingLog();
                        log.PacketType = PacketType.Data;
                        log.IsSend = true;
                        log.NodeID = this.ID;
                        log.Operation = "To:" + Reciver.ID;
                        log.Time = DateTime.Now;
                        log.Distance_M = Distance_M;
                        log.UsedEnergy_Nanojoule = UsedEnergy_Nanojoule;
                        log.RemaimBatteryEnergy_Joule = ResidualEnergy;
                        log.PID = packt.PID;
                        this.Logs.Add(log);
                    }

                    // for control packet.
                    if (packt.PacketType == PacketType.Control)
                    {
                        // just to remember how much energy is consumed here.
                        PublicParamerters.EnergyComsumedForControlPackets += UsedEnergy_joule;
                    }
                }

                if (Settings.Default.ShowRoutingPaths)
                {
                    OpenChanel(Reciver.ID, packt.PID);
                }

            }
            else if (enCon == EnergyConsumption.Recive)
            {

                double UsedEnergy_Nanojoule = EnergyModel.Receive(packt.PacketLength);
                double UsedEnergy_joule = ConvertToJoule(UsedEnergy_Nanojoule);
                ResidualEnergy = ResidualEnergy - UsedEnergy_joule;
                packt.UsedEnergy_Joule += UsedEnergy_joule;
                PublicParamerters.TotalEnergyConsumptionJoule += UsedEnergy_joule;


                if (packt.PacketType == PacketType.Control)
                {
                    // just to remember how much energy is consumed here.
                    PublicParamerters.EnergyComsumedForControlPackets += UsedEnergy_joule;
                }


            }

        }


        /// <summary>
        ///  data or control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reciver"></param>
        /// <param name="packt"></param>
        public void ReceivePacket(Packet packt)
        {
            packt.Path += ">" + ID;
            if (packt.Destination.ID == ID)
            {
                packt.isDelivered = true;
                PublicParamerters.NumberofDeliveredPacket += 1;
                PublicParamerters.FinishedRoutedPackets.Add(packt);// should we add it to the packet which should be store in the sink?
                Console.WriteLine("PID:" + packt.PID + " has been delivered.");

                ComputeOverhead(packt, EnergyConsumption.Recive, null);

                MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_total_consumed_energy.Content = PublicParamerters.TotalEnergyConsumptionJoule + " (JOULS)", DispatcherPriority.Send);
                MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_Number_of_Delivered_Packet.Content = PublicParamerters.NumberofDeliveredPacket, DispatcherPriority.Send);
                MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_sucess_ratio.Content = PublicParamerters.DeliveredRatio, DispatcherPriority.Send);
                MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_nymber_inQueu.Content = PublicParamerters.InQueuePackets.ToString());
                if (packt.PacketType == PacketType.Control)
                    UnIdentifyEndNode(packt.Destination);
                if (packt.PacketType == PacketType.Data)
                    UnIdentifySourceNode(packt.Source);

            }
            else
            {
                if (packt.Hops > packt.TimeToLive)
                {
                    // drop the paket.
                    PublicParamerters.NumberofDropedPacket += 1;
                    packt.isDelivered = false;
                    PublicParamerters.FinishedRoutedPackets.Add(packt);
                    Console.WriteLine("PID:" + packt.PID + " has been droped.");
                    MainWindow.Dispatcher.Invoke(() => MainWindow.lbl_Number_of_Droped_Packet.Content = PublicParamerters.NumberofDropedPacket, DispatcherPriority.Send);
                }
                else
                {
                    // forward the packet.
                    this.SendPacekt(packt);
                }
            }
        }


        #endregion







        private void lbl_MouseEnter(object sender, MouseEventArgs e)
        {
            ToolTip = new Label() { Content = "("+ID + ") [ " + ResidualEnergyPercentage + "% ] [ " + ResidualEnergy + " J ]" };
        }

        private void btn_show_routing_log_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Logs.Count>0)
            {
                UiShowRelativityForAnode re = new ui.UiShowRelativityForAnode();
                re.dg_relative_shortlist.ItemsSource = Logs;
                re.Show();
            }
        }

        private void btn_draw_random_numbers_MouseDown(object sender, MouseButtonEventArgs e)
        {
            List<KeyValuePair<int, double>> rands = new List<KeyValuePair<int, double>>();
            int index = 0;
            foreach (RoutingLog log in Logs )
            {
                if(log.IsSend)
                {
                    index++;
                    rands.Add(new KeyValuePair<int, double>(index, log.ForwardingRandomNumber));
                }
            }
            UiRandomNumberGeneration wndsow = new ui.UiRandomNumberGeneration();
            wndsow.chart_x.DataContext = rands;
            wndsow.Show();
        }

        private void Ellipse_center_MouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void btn_show_my_duytcycling_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void btn_draw_paths_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NetworkVisualization.UpLinksDrawPaths(this);
        }

       
         
        private void btn_show_my_flows_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
            ListControl ConMini = new ui.conts.ListControl();
            ConMini.lbl_title.Content = "Mini-Flow-Table";
            ConMini.dg_date.ItemsSource = MiniFlowTable;


            ListControl ConNei = new ui.conts.ListControl();
            ConNei.lbl_title.Content = "Neighbors-Table";
            ConNei.dg_date.ItemsSource = NeighborsTable;

            UiShowLists win = new UiShowLists();
            win.stack_items.Children.Add(ConMini);
            win.stack_items.Children.Add(ConNei);
            win.Title = "Tables of Node " + ID;
            win.Show();
            win.WindowState = WindowState.Maximized;
        }

        private void btn_send_1_p_each1sec_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SendPacketTimer.Start();
            SendPacketTimer.Tick += SendPacketTimer_Random; // redfine th trigger.
        }



        public void RandomSelectEndNodes(int numOFpACKETS)
        {
            if (PublicParamerters.SimulationTime > PublicParamerters.MacStartUp)
            {
                int index = 1 + Convert.ToInt16(UnformRandomNumberGenerator.GetUniform(PublicParamerters.NumberofNodes - 2));
                if (index != PublicParamerters.SinkNode.ID)
                {
                    Sensor endNode = MainWindow.myNetWork[index];
                    GenerateMultipleControlPackets(numOFpACKETS, endNode);
                }
            }
        }

        private void SendPacketTimer_Random(object sender, EventArgs e)
        {
            if (ID != PublicParamerters.SinkNode.ID)
            {
                // uplink:
                GenerateMultipleDataPackets(1);
            }
            else
            { //
                RandomSelectEndNodes(1);
            }
        }

        /// <summary>
        /// i am slected as end node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_select_me_as_end_node_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Label lbl_title = sender as Label;
            switch (lbl_title.Name)
            {
                case "Btn_select_me_as_end_node_1":
                    {
                       PublicParamerters.SinkNode.GenerateMultipleControlPackets(1, this);

                        break;
                    }
                case "Btn_select_me_as_end_node_10":
                    {
                        PublicParamerters.SinkNode.GenerateMultipleControlPackets(10, this);
                        break;
                    }
                //Btn_select_me_as_end_node_1_5sec

                case "Btn_select_me_as_end_node_1_5sec":
                    {
                        PublicParamerters.SinkNode.SendPacketTimer.Start();
                        PublicParamerters.SinkNode.SendPacketTimer.Tick += SelectMeAsEndNodeAndSendonepacketPer5s_Tick;
                        break;
                    }
            }
        }

        public void SelectMeAsEndNodeAndSendonepacketPer5s_Tick(object sender, EventArgs e)
        {
            PublicParamerters.SinkNode.GenerateMultipleControlPackets(1, this);
        }





        /*** Vistualize****/

        public void ShowID(bool isVis )
        {
            if (isVis) { lbl_Sensing_ID.Visibility = Visibility.Visible; lbl_hops_to_sink.Visibility = Visibility.Visible; }
            else { lbl_Sensing_ID.Visibility = Visibility.Hidden; lbl_hops_to_sink.Visibility = Visibility.Hidden; }
        }

        public void ShowSensingRange(bool isVis)
        {
            if (isVis) Ellipse_Sensing_range.Visibility = Visibility.Visible;
            else Ellipse_Sensing_range.Visibility = Visibility.Hidden;
        }

        public void ShowComunicationRange(bool isVis)
        {
            if (isVis) Ellipse_Communication_range.Visibility = Visibility.Visible;
            else Ellipse_Communication_range.Visibility = Visibility.Hidden;
        }

        public void ShowBattery(bool isVis) 
        {
            if (isVis) Prog_batteryCapacityNotation.Visibility = Visibility.Visible;
            else Prog_batteryCapacityNotation.Visibility = Visibility.Hidden;
        }

        private void btn_update_mini_flow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UplinkRouting.UpdateUplinkFlowEnery(this);
        }
    }
}
