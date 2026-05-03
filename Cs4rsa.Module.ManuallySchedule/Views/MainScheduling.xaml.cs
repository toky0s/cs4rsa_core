using Cs4rsa.Database.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Views;
using Cs4rsa.Module.ManuallySchedule.Models;
using Cs4rsa.Module.ManuallySchedule.ViewModels;
using Cs4rsa.Module.Shared;

using Prism.Regions;

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Module.ManuallySchedule.Views
{
    public partial class MainScheduling
    {
        public MainScheduling(
            IRegionManager regionManager
        )
        {
            InitializeComponent();

            regionManager.RegisterViewWithRegion(RegionInfo.Manual_ClassGroup, typeof(Clg));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Choose, typeof(Choose));
            regionManager.RegisterViewWithRegion(RegionInfo.Manual_Scheduler, typeof(Scheduler));
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
    }
}
