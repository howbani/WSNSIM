using MiniSDN.Intilization;
using MiniSDN.Dataplane;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MiniSDN.Dataplane.PacketRouter;

namespace MiniSDN.ControlPlane.NOS.TC
{
    public class TopologyConstractor
    {
        /// <summary>
        /// build and conscrut the topology.
        /// assume that all nodes has send theeir locations and their neighboring list.
        /// </summary>
        /// <param name="Mynetwork"></param>
        public static void BuildToplogy( Canvas Canvas_SensingFeild, List<Sensor> Mynetwork)
        {
            if (Mynetwork != null)
            {
                if (Mynetwork.Count > 0)
                {
                    foreach (Sensor fromSen in Mynetwork)
                    {
                        if(fromSen.NeighborsTable != null)
                        {
                            fromSen.MyArrows.Clear(); // remove in each computation.
                            if (fromSen.NeighborsTable.Count>0)
                            {
                                Point startFrom = fromSen.CenterLocation;
                                foreach (NeighborsTableEntry  toSen in fromSen.NeighborsTable)
                                {
                                    Point endBy = toSen.CenterLocation;
                                    Arrow arr = new Arrow();
                                    arr.To = toSen.NeiNode;
                                    arr.From = fromSen;
                                  //  arr.PacketAnimator = AnimatePacket.GetStoryboard(sen, nei);
                                    arr.Stroke = Brushes.Gray;
                                    arr.StrokeThickness = 0.2;
                                    arr.X1 = startFrom.X;
                                    arr.Y1 = startFrom.Y;
                                    arr.X2 = endBy.X;
                                    arr.Y2 = endBy.Y;
                                    arr.HeadHeight = 0.2;
                                    arr.HeadWidth = 0.2;
                                    Canvas_SensingFeild.Children.Add(arr);
                                    if (Properties.Settings.Default.ShowArrowsIntializing) arr.Visibility = Visibility.Visible; else arr.Visibility = Visibility.Hidden;
                                    fromSen.MyArrows.Add(arr);
                                }
                            }
                        }
                    }
                }
            }
           
        }

    }
}
