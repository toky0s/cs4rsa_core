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
        public ScheduleTable()
        {
            InitializeComponent();
            //DataGridFirstPhase.ItemsSource = scheduleTableViewModel.Schedule1;
            //DataGridSecondPhase.ItemsSource = scheduleTableViewModel.Schedule2;
        }
    }
}