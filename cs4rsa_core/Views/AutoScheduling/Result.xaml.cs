using System.Windows.Controls;

namespace Cs4rsa.Views.AutoScheduling
{
    public partial class Result : UserControl
    {
        public Result()
        {
            InitializeComponent();
        }


        //private void CheckBoxHideConflict_Click(object sender, RoutedEventArgs e)
        //{
        //    CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource).Refresh();
        //}

        //private void CheckBoxHideConflictTime_Click(object sender, RoutedEventArgs e)
        //{
        //    CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource).Refresh();
        //}

        //private void CheckBoxHideCannotSimulate_Click(object sender, RoutedEventArgs e)
        //{
        //    CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource).Refresh();
        //}

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
        //    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewCombinationModels.ItemsSource);
        //    view.Filter = CombinationFilter;
        //}


        //private bool CombinationFilter(object obj)
        //{
        //    CombinationModel combinationModel = obj as CombinationModel;
        //    return CheckCannotShowFilter(combinationModel) && CheckConflict(combinationModel) && CheckConflictPlace(combinationModel);
        //}
    }
}
