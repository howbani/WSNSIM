using MiniSDN.Intilization;
using MiniSDN.Forwarding;
using System;
using System.Windows.Threading;
using static MiniSDN.Dataplane.PublicParamerters;
using System.Windows.Shapes;
using System.Windows.Media;

namespace MiniSDN.Dataplane
{

    /// <summary>
    /// implementation of BoxMAC.
    /// Ammar Hawbani.
    /// </summary>
    public class BoXMAC: Shape
    {
        /// <summary>
        /// in sec.
        /// </summary>
      

        private Sensor Node; // the node that runs the BoxMac

        // this timer to swich on the sensor, when to start. after swiching on this sensor, this timer will be stoped.
        private DispatcherTimer SwichOnTimer = new DispatcherTimer();// ashncrous swicher.

        // the timer to swich between the sleep and active states.
        private DispatcherTimer ActiveSleepTimer = new DispatcherTimer();

       
        private int ActiveCounter = 0;
        private int SleepCounter = 0;

        protected override Geometry DefiningGeometry
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// intilize the MAC
        /// </summary>
        /// <param name="_Node"></param>
        public BoXMAC(Sensor _Node)
        {
            Node = _Node;
            if (Node != null)
            {
                if (Node.ID != PublicParamerters.SinkNode.ID)
                {
                    double xpasn = 1 + UnformRandomNumberGenerator.GetUniformSleepSec(MacStartUp);
                    // the swich on timer.
                    SwichOnTimer.Interval = TimeSpan.FromSeconds(xpasn);
                    SwichOnTimer.Start();
                    SwichOnTimer.Tick += ASwichOnTimer_Tick;
                    ActiveCounter = 0;
                    // active/sleep timer:
                    ActiveSleepTimer.Interval = TimeSpan.FromSeconds(1);
                    ActiveSleepTimer.Tick += ActiveSleepTimer_Tick; ;
                    SleepCounter = 0;

                    // intialized:
                    Node.CurrentSensorState = SensorState.intalized;
                    Node.Ellipse_MAC.Fill = NodeStateColoring.IntializeColor;
                }
                else
                {
                    // the
                    PublicParamerters.SinkNode.CurrentSensorState = SensorState.Active;
                }
            }
        }

        private void ActiveSleepTimer_Tick(object sender, EventArgs e)
        {
           // lock (Node)
            {
                if (Node.CurrentSensorState == SensorState.Active)
                {
                    ActiveCounter = ActiveCounter + 1;
                    if (ActiveCounter == 1)
                    {

                        Action x = () => Node.Ellipse_MAC.Fill = NodeStateColoring.ActiveColor;
                        Dispatcher.Invoke(x);
                    }
                    else if (ActiveCounter > Periods.ActivePeriod)
                    {
                        ActiveCounter = 0;
                        SleepCounter = 0;
                        Node.CurrentSensorState = SensorState.Sleep;
                    }
                }
                else if (Node.CurrentSensorState == SensorState.Sleep)
                {
                    SleepCounter = SleepCounter + 1;
                    if (SleepCounter == 1)
                    {
                        Action x = () => Node.Ellipse_MAC.Fill = NodeStateColoring.SleepColor;
                        Dispatcher.Invoke(x);

                        // Node.DutyCycleString.Add(PublicParamerters.SimulationTime);
                    }
                    else if (SleepCounter > Periods.SleepPeriod)
                    {
                        ActiveCounter = 0;
                        SleepCounter = 0;
                        Node.CurrentSensorState = SensorState.Active;
                    }
                }
            }

            /*
            //: Test. 
            if(Node.ID==1)
            {
                Console.WriteLine("NID: 61 State: " + Node.CurrentSensorState.ToString() + " ActiveCounter=" + ActiveCounter + " SleepCounter=" + SleepCounter);
            }*/

        }

        /// <summary>
        /// reset active counter.
        /// </summary>
        public void SwichToActive()
        {
            if (Node.ID != PublicParamerters.SinkNode.ID)
            {
                if (Node.CurrentSensorState == SensorState.Sleep)
                {
                    Dispatcher.Invoke(() => Node.CurrentSensorState = SensorState.Active, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => SleepCounter = 0, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => ActiveCounter = 0, DispatcherPriority.Send);
                }
                else
                {
                    ActiveCounter = 0;
                }
            }
        }

        /// <summary>
        /// re set sleep counter.
        /// </summary>
        public void SwichToSleep()
        {
            if (Node.ID != PublicParamerters.SinkNode.ID)
            {
                if (Node.CurrentSensorState == SensorState.Active)
                {
                    Dispatcher.Invoke(() => Node.CurrentSensorState = SensorState.Sleep, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => SleepCounter = 0, DispatcherPriority.Send);
                    Dispatcher.Invoke(() => ActiveCounter = 0, DispatcherPriority.Send); ;
                }
                else
                {
                    SleepCounter = 0;
                }
            }
        }

        /// <summary>
        /// run the timer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ASwichOnTimer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() => ActiveSleepTimer.Start(),DispatcherPriority.Send);
            Dispatcher.Invoke(() => Node.CurrentSensorState = SensorState.Active, DispatcherPriority.Send);
            Dispatcher.Invoke(() => Node.Ellipse_MAC.Fill = NodeStateColoring.ActiveColor, DispatcherPriority.Send);
            Dispatcher.Invoke(() => SwichOnTimer.Interval = TimeSpan.FromSeconds(0), DispatcherPriority.Send);
            Dispatcher.Invoke(() => SwichOnTimer.Stop(), DispatcherPriority.Send);// stop me
        }


    }
}
