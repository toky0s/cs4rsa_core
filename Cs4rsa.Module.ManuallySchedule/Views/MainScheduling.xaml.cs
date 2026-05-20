using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.ViewModels;
using Cs4rsa.Module.Shared;
using Cs4rsa.UI.ScheduleTable.CustomControls;

using Prism.Regions;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using static Cs4rsa.UI.ScheduleTable.CustomControls.ScheduleBlock;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class MainScheduling
    {
        public MainScheduling(
            IRegionManager regionManager
        )
        {
            InitializeComponent();

            //regionManager.RegisterViewWithRegion(RegionInfo.Manual_ClassGroup, typeof(Clg));
            //regionManager.RegisterViewWithRegion(RegionInfo.Manual_Choose, typeof(Choose));
            //regionManager.RegisterViewWithRegion(RegionInfo.Manual_Scheduler, typeof(Scheduler));

            // Xử lý sự kiện người dùng unselect một class group từ Schedule View
            AddHandler(ScheduleBlock.UnselectClassGroupEvent, new RoutedEventHandler(OnUnselectClassGroup));
        }

        private void OnUnselectClassGroup(object sender, RoutedEventArgs e)
        {
            var args = (UnselectClassGroupEventArgs)e;
            ((MainSchedulingViewModel)DataContext).UnSelectClassGroupCommand.Execute(args.ClassGroupName);
        }

        private void DataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                var grid = sender as DataGrid;
                var itemsToDelete = grid.SelectedItems.Cast<ClassGroupModel>().ToList();

                if (itemsToDelete.Any())
                {
                    ((MainSchedulingViewModel)DataContext).RemoveSelectedCommand.Execute(itemsToDelete);
                    e.Handled = true; // Ngăn DataGrid tự xử lý mặc định
                }
            }
        }

        private void DataGrid_SelectedClassGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.AddedItems.Cast<ClassGroupModel>().ToList().ForEach(item =>
            {
                ((MainSchedulingViewModel)DataContext).RemoveSelectedCommand.RaiseCanExecuteChanged();
            });
        }
    }
}
