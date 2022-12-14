using Cs4rsa.Models;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cs4rsa.Views.AutoScheduling
{
    public partial class Auto : UserControl
    {
        public Auto()
        {
            InitializeComponent();
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource);
            view.Filter = CombinationFilter;
        }
    }
}
