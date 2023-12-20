using Cs4rsa.App.Events.TopMenuEvents;

using DryIoc;

using Prism.Events;
using Prism.Regions;

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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cs4rsa.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();

            regionManager.RegisterViewWithRegion(RegionInfo.TopMenu, typeof(TopMenu));
            regionManager.RegisterViewWithRegion(RegionInfo.Home, typeof(Home));
        }
    }
}
