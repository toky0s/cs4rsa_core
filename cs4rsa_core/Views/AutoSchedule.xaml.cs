using cs4rsa_core.Models;
using cs4rsa_core.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace cs4rsa_core.Views
{
    public partial class AutoSchedule : UserControl
    {
        public AutoSchedule()
        {
            InitializeComponent();
            treeView.ItemsSource = (DataContext as AutoScheduleViewModel).ProgramFolderModels;
            QuangTrung.IsChecked = true;
            HoaKhanhNam.IsChecked = true;
            NguyenVanLinh137.IsChecked = true;
            NguyenVanLinh254.IsChecked = true;
            PhanThanh.IsChecked = true;
            VietTin.IsChecked = true;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource);
            view.Filter = CombinationFilter;
        }

        private bool CombinationFilter(object obj)
        {
            if (CheckBoxHideConflict.IsChecked == true)
            {
                CombinationModel combinationModel = obj as CombinationModel;
                return !combinationModel.IsConflict;
            }
            return true;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window window = sender as Window;
            window.Topmost = true;
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

        private void All_Checked(object sender, RoutedEventArgs e)
        {
            if (QuangTrung != null &&
            HoaKhanhNam != null &&
            NguyenVanLinh137 != null &&
            NguyenVanLinh254 != null &&
            PhanThanh != null &&
            VietTin != null)
            {
                QuangTrung.IsChecked = true;
                HoaKhanhNam.IsChecked = true;
                NguyenVanLinh137.IsChecked = true;
                NguyenVanLinh254.IsChecked = true;
                PhanThanh.IsChecked = true;
                VietTin.IsChecked = true;
            }
        }

        private void All_Unchecked(object sender, RoutedEventArgs e)
        {
            if (QuangTrung != null &&
            HoaKhanhNam != null &&
            NguyenVanLinh137 != null &&
            NguyenVanLinh254 != null &&
            PhanThanh != null &&
            VietTin != null)
            {
                QuangTrung.IsChecked = false;
                HoaKhanhNam.IsChecked = false;
                NguyenVanLinh137.IsChecked = false;
                NguyenVanLinh254.IsChecked = false;
                PhanThanh.IsChecked = false;
                VietTin.IsChecked = false;
            }
        }

        private void CheckBoxHideConflict_Click(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource).Refresh();
        }
    }
}
