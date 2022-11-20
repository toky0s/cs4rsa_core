using Cs4rsa.Models;
using Cs4rsa.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cs4rsa.Views
{
    public partial class AutoSchedule : UserControl
    {
        public AutoSchedule()
        {
            InitializeComponent();
            treeView.ItemsSource = (DataContext as AutoScheduleViewModel).ProgramFolderModels;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource);
            view.Filter = CombinationFilter;
        }

        private bool CombinationFilter(object obj)
        {
            CombinationModel combinationModel = obj as CombinationModel;
            return CheckCannotShowFilter(combinationModel) && CheckConflict(combinationModel) && CheckConflictPlace(combinationModel);
        }

        private bool CheckCannotShowFilter(CombinationModel combinationModel)
        {
            if (CheckBoxHideCannotSimulate.IsChecked == true)
            {
                return combinationModel.IsCanShow;
            }
            return true;
        }

        private bool CheckConflict(CombinationModel combinationModel)
        {
            if (CheckBoxHideConflict.IsChecked == true)
            {
                return !combinationModel.IsHaveTimeConflicts();
            }
            return true;
        }

        private bool CheckConflictPlace(CombinationModel combinationModel)
        {
            if (CheckBoxHideConflictPlace.IsChecked == true)
            {
                return !combinationModel.IsHavePlaceConflicts();
            }
            return true;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as AutoScheduleViewModel).SelectedProSubject = e.NewValue is ProgramSubjectModel ? e.NewValue as ProgramSubjectModel : null;
        }

        // Chống scroll auto đưa item vào trung tâm khi focus
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void CheckBoxHideConflict_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource).Refresh();
        }

        private void CheckBoxHideConflictTime_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource).Refresh();
        }

        private void CheckBoxHideCannotSimulate_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource).Refresh();
        }
    }
}
