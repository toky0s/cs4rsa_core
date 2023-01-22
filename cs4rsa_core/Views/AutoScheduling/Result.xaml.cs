using System.Windows.Controls;

namespace Cs4rsa.Views.AutoScheduling
{
    public partial class Result : UserControl
    {
        public Result()
        {
            InitializeComponent();
        }

        private void TextBox_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int crrVal = int.Parse(tb.Text);
            int nextVal = 0;
            if (e.Delta >= 120)
            {
                nextVal = crrVal + 1;

            }
            else if (e.Delta < 0 && crrVal > 0)
            {
                nextVal = crrVal - 1;
            }
            tb.Text = nextVal.ToString();
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
