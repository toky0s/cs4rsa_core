using System;
using System.Collections.Generic;
using System.Globalization;
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
using cs4rsa.Helpers;

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
            DataGridFirstPhase.ItemsSource = scheduleTableViewModel.Phase1Schedule;
            DataGridSecondPhase.ItemsSource = scheduleTableViewModel.Phase2Schedule;
        }
    }
}

namespace cs4rsa.Converters
{
    public class ShortedTimeViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ShortedTime shortedTime = value as ShortedTime;
            return shortedTime.NewTime.ToString("HH:mm");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}