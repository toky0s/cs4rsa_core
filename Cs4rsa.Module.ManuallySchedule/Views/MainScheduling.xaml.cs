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

            // Xử lý sự kiện người dùng unselect một class group từ Schedule View
            AddHandler(UnselectClassGroupEvent, new RoutedEventHandler(OnUnselectClassGroup));

            // Init sort
            SortByName.IsChecked = true;
            SortAscending.IsChecked = true;
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

        // ================================ Sort classes ================================ //
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị context menu khi click trái
            SortButton.ContextMenu.PlacementTarget = SortButton;
            SortButton.ContextMenu.IsOpen = true;
        }

        private void SortField_Click(object sender, RoutedEventArgs e)
        {
            // Bỏ chọn các field khác
            foreach (var item in new[] { SortByName, SortBySlot })
                item.IsChecked = false;

            // Đánh dấu item được chọn
            var clicked = sender as MenuItem;
            clicked.IsChecked = true;
        }

        private void SortOrder_Click(object sender, RoutedEventArgs e)
        {
            // Bỏ chọn các order khác
            foreach (var item in new[] { SortAscending, SortDescending })
                item.IsChecked = false;

            // Đánh dấu item được chọn
            var clicked = sender as MenuItem;
            clicked.IsChecked = true; 
        }

        private void ListBox_ClassGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lb = (ListBox)sender;
            if (lb.SelectedItem != null)
            {
                lb.ScrollIntoView(lb.SelectedItem);
            }
        }
    }
}
