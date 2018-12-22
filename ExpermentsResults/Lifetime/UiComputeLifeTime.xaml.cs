using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MiniSDN.Dataplane;
using MiniSDN.Dataplane;
using System;
using MiniSDN.Properties;
using System.Windows.Threading;
using MiniSDN.ui;

namespace MiniSDN.ExpermentsResults.Lifetime
{
    public enum Stategy { all,some };
    /// <summary>
    /// Interaction logic for UiComputeLifeTimeScen1.xaml
    /// </summary>
    public partial class UiComputeLifeTime : Window
    {
        public Stategy myStategy;
        public DispatcherTimer CounterTimer = new DispatcherTimer();
        int NOD = 0;
        int NOS = 0;
        int NOP = 0;
        NetworkLifeTime Ran = new NetworkLifeTime();
        List<Sensor> myNetWork;
        MainWindow mainWindow; 
        public UiComputeLifeTime(MainWindow _mainWindow) 
        {
            InitializeComponent();
            mainWindow = _mainWindow;


            CounterTimer.Tick += CounterTimer_Tick;
            myNetWork = _mainWindow.myNetWork;
            for (int i=1;i< myNetWork.Count;i++) 
            {
                com_nos.Items.Add(new ComboBoxItem() { Content=i.ToString() });
                com_nop.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                com_NOD.Items.Add(new ComboBoxItem() { Content = i.ToString() });
                com_num_of_deadNodes.Items.Add(new ComboBoxItem() { Content = i.ToString() });
            }

            com_nos.Text = "10";
            com_nop.Text = "1";
            com_NOD.Text = "1";
            com_num_of_deadNodes.Text = "1";


          

            for (int i = 5; i <= 15; i++)
            {
                comb_startup.Items.Add(i);
            }
            comb_startup.Text = "10";

            for (int i = 1; i <= 5; i++)
            {
                comb_active.Items.Add(i);
                comb_sleep.Items.Add(i);
            }
            comb_active.Text = "1";
            comb_sleep.Text = "2";



            int conrange = 5;
            for (int i = 0; i <= conrange; i++)
            {
                if (i == conrange)
                {
                    double dc = Convert.ToDouble(i);
                    com_direction.Items.Add(dc);
                    com_energy.Items.Add(dc);
                    com_prependicular.Items.Add(dc);
                    com_transmision_distance.Items.Add(dc);
                }
                else
                {
                    for (int j = 0; j <= 9; j++)
                    {
                        string str = i + "." + j;
                        double dc = Convert.ToDouble(str);
                        com_direction.Items.Add(dc);
                        com_energy.Items.Add(dc);
                        com_prependicular.Items.Add(dc);
                        com_transmision_distance.Items.Add(dc);

                    }
                }
            }

            // set defuals:
            com_direction.Text = Settings.Default.ExpoHCnt.ToString();
            com_energy.Text = Settings.Default.ExpoLCnt.ToString();
            com_prependicular.Text = Settings.Default.ExpoHCnt.ToString();
            com_transmision_distance.Text = Settings.Default.ExpoRCnt.ToString();


            for (int i = 1; i <= 100; i++)
            {
                comb_update.Items.Add(i);
            }
            comb_update.Text = "5";

        }

        int round = 0;
        private void CounterTimer_Tick(object sender, EventArgs e)
        {
            if (myStategy == Stategy.some)
            {
                round++;
                mainWindow.lbl_rounds.Content = round.ToString();
                this.Title = round + " #";
                if (PublicParamerters.DeadNodeList.Count < NOD)
                {
                    Ran.RandimSelect(myNetWork, NOS, NOP);
                    PublicParamerters.Rounds = round;
                    PublicParamerters.FinishedRoutedPackets.Clear();


                }
                else
                {
                    CounterTimer.Stop();
                    CounterTimer.Interval = TimeSpan.FromSeconds(0);
                    mainWindow.TimerCounter.Stop();
                    mainWindow.top_menu.IsEnabled = true;
                    this.Close();
                }
            }
            else
            {
                round++;
                mainWindow.lbl_rounds.Content = round.ToString();
                this.Title = round + " #";
                if (PublicParamerters.DeadNodeList.Count < NOD)
                {
                    Ran.FromAllNodes(myNetWork);
                    PublicParamerters.Rounds = round;
                    PublicParamerters.FinishedRoutedPackets.Clear();
                }
                else
                {
                    CounterTimer.Stop();
                    CounterTimer.Interval = TimeSpan.FromSeconds(0);
                    mainWindow.TimerCounter.Stop();
                    mainWindow.top_menu.IsEnabled = true;
                    this.Close();
                }
            }


        }


        private void btn_compute_life_time_Click(object sender, RoutedEventArgs e)
        {
           
            myStategy = Stategy.some;
            PublicParamerters.SimulationTime = 0;
            CounterTimer.Interval = TimeSpan.FromSeconds(1);
            PublicParamerters.UpdateLossPercentage = Convert.ToInt16(comb_update.Text);
            Settings.Default.DrawPacketsLines = Convert.ToBoolean(chk_drawrouts.IsChecked);
            Settings.Default.KeepLogs = Convert.ToBoolean(chk_save_logs.IsChecked);

            Settings.Default.ExpoHCnt = Convert.ToDouble(com_prependicular.Text);
            Settings.Default.ExpoRCnt = Convert.ToDouble(com_transmision_distance.Text);
            Settings.Default.ExpoLCnt = Convert.ToDouble(com_energy.Text);
            Settings.Default.ExpoHCnt = Convert.ToDouble(com_direction.Text);
            Settings.Default.DrawPacketsLines = Convert.ToBoolean(chk_drawrouts.IsChecked);
            Settings.Default.KeepLogs = Convert.ToBoolean(chk_save_logs.IsChecked);
            NOD = Convert.ToInt16(com_NOD.Text);
            NOS = Convert.ToInt16(com_nos.Text);
            NOP = Convert.ToInt16(com_nop.Text);
            PublicParamerters.NOS = NOS;
            PublicParamerters.NOP = NOP;
            mainWindow.PacketRate = NOS.ToString() + " nodes/" + NOP.ToString() + "pck/round";
            mainWindow.top_menu.IsEnabled = false;
            this.IsEnabled = false;

            CounterTimer.Start();

            this.Hide();
        }

        private void btn_from_all_Click(object sender, RoutedEventArgs e)
        {

            myStategy = Stategy.all;
            PublicParamerters.SimulationTime = 0;
            CounterTimer.Interval = TimeSpan.FromSeconds(5);
            PublicParamerters.UpdateLossPercentage = Convert.ToInt16(comb_update.Text);
            Settings.Default.DrawPacketsLines = Convert.ToBoolean(chk_drawrouts.IsChecked);
            Settings.Default.KeepLogs = Convert.ToBoolean(chk_save_logs.IsChecked);

            Settings.Default.ExpoHCnt = Convert.ToDouble(com_prependicular.Text);
            Settings.Default.ExpoRCnt = Convert.ToDouble(com_transmision_distance.Text);
            Settings.Default.ExpoLCnt = Convert.ToDouble(com_energy.Text);
            Settings.Default.ExpoHCnt = Convert.ToDouble(com_direction.Text);
            Settings.Default.DrawPacketsLines = Convert.ToBoolean(chk_drawrouts.IsChecked);
            Settings.Default.KeepLogs = Convert.ToBoolean(chk_save_logs.IsChecked);

            NOD = Convert.ToInt16(com_num_of_deadNodes.Text);
            PublicParamerters.NOS = myNetWork.Count;
            PublicParamerters.NOP = 1;
            mainWindow.PacketRate = NOS.ToString() + " nodes/" + NOP.ToString() + "pck/round";

            mainWindow.top_menu.IsEnabled = false;
            this.IsEnabled = false;

            CounterTimer.Start();
            this.Hide();
        }

        private void comb_startup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_startup.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            Settings.Default.MacStartUp = va;
        }

        private void comb_active_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_active.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            Settings.Default.ActivePeriod = va;
        }

        private void comb_sleep_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            object objval = comb_sleep.SelectedItem as object;
            int va = Convert.ToInt16(objval);
            Settings.Default.SleepPeriod = va;
        }


        
    }
}
