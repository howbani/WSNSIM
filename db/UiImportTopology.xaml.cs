using MiniSDN.Dataplane;
using MiniSDN.ui;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MiniSDN.db
{
    /// <summary>
    /// Interaction logic for UiImportTopology.xaml
    /// </summary>
    public partial class UiImportTopology : Window
    {

        public UiImportTopology(MainWindow MainWindow)
        {
            InitializeComponent();

           foreach( NetwokImport net in NetworkTopolgy.ImportNetworkNames(this))
           {
                net.MainWindow = MainWindow;
                stk_netwoks.Children.Add(net);
           }
        }
    }
}
