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
using cs4rsa.ViewModels;

namespace cs4rsa.Views
{
    /// <summary>
    /// Interaction logic for ScheduleTable.xaml
    /// </summary>
    public partial class ScheduleTable : UserControl
    {
        private ScheduleTableViewModel scheduleTableViewModel = new ScheduleTableViewModel();
        public ScheduleTable()
        {
            InitializeComponent();
            DataGridFirstPhase.DataContext = scheduleTableViewModel.Phase1Schedule.DefaultView;
            DataGridSecondPhase.DataContext = scheduleTableViewModel.Phase2Schedule.DefaultView;
        }
    }
}
