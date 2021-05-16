using cs4rsa.Helpers;
using cs4rsa.Models;
using cs4rsa.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

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
            DataGridFirstPhase.ItemsSource = scheduleTableViewModel.Schedule1;
            DataGridSecondPhase.ItemsSource = scheduleTableViewModel.Schedule2;
        }

        /// <summary>
        /// Phương thức này thực hiện tô màu cho bảng sau khi render xong dữ liệu
        /// </summary>
        /// <param name="scheduleRows"></param>
        /// <param name="dataGrid"></param>
        private void PaintColor(ObservableCollection<ScheduleRow> scheduleRows, DataGrid dataGrid)
        {

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

    public class ClassGroupModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is ClassGroupModel classGroupModel ? classGroupModel.Name : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}