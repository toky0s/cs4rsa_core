using cs4rsa.BasicData;
using cs4rsa.Enums;
using cs4rsa.Models;
using cs4rsa.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace cs4rsa.Views
{
    /// <summary>
    /// Interaction logic for CombinationContainerWindow.xaml
    /// </summary>
    public partial class CombinationContainerWindow : Window
    {
        private List<CheckBox> _checkBoxes;
        private AutoScheduleViewModel _vm;

        public CombinationContainerWindow(List<CombinationModel> combinationModels, object dataContext)
        {
            InitializeComponent();
            _vm = dataContext as AutoScheduleViewModel;
            DataContext = _vm;
            //_combinationViewModel = new CombinationViewModel(combinationModels);
            _checkBoxes = new List<CheckBox>() {
                         Mon_Aft, Mon_Mor, Mon_Nig,
                         Tus_Aft, Tus_Mor, Tus_Nig,
                         Wed_Aft, Wed_Mor, Wed_Nig,
                         Thur_Aft,Thur_Mor, Thur_Nig,
                         Fri_Aft, Fri_Mor, Fri_Nig,
                         Sat_Aft, Sat_Mor, Sat_Nig,
                         Sun_Aft, Sun_Mor, Sun_Nig};
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_vm.CombinationModels);
            view.Filter = Filter;
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        private bool Filter(object obj)
        {
            CombinationModel combinationModel = obj as CombinationModel;
            List<CheckBox> checkBoxs = GetCheckedCheckBoxs();
            foreach (CheckBox checkBox in checkBoxs)
            {
                Tuple<DayOfWeek, Session> tuplePara = CheckBoxNameToDayOfWeekAndSession(checkBox.Name);
                if (combinationModel.GetSchedule().GetLearnState(tuplePara.Item1, tuplePara.Item2) != LearnState.Free)
                    return false;
            }
            return true;
        }

        private Tuple<DayOfWeek, Session> CheckBoxNameToDayOfWeekAndSession(string checkboxName)
        {
            Dictionary<string, DayOfWeek> nameAndDay = new Dictionary<string, DayOfWeek>()
            {
                { "Mon", DayOfWeek.Monday },
                { "Tus", DayOfWeek.Tuesday },
                { "Wed", DayOfWeek.Wednesday },
                { "Thur", DayOfWeek.Thursday },
                { "Fri", DayOfWeek.Friday },
                { "Sat", DayOfWeek.Saturday },
                { "Sun", DayOfWeek.Sunday },
            };

            Dictionary<string, Session> nameAndSession = new Dictionary<string, Session>()
            {
                { "Mor", Session.Morning },
                { "Aft", Session.Afternoon },
                { "Nig", Session.Night },
            };
            string[] nameSlices = checkboxName.Split(new char[] { '_' });
            string nameDay = nameSlices[0];
            string nameSession = nameSlices[1];
            return Tuple.Create(nameAndDay[nameDay], nameAndSession[nameSession]);
        }

        private List<CheckBox> GetCheckedCheckBoxs()
        {
            return _checkBoxes.Where(x => x.IsChecked == true).ToList();
        }

        private void Reload(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(_vm.CombinationModels).Refresh();
        }

        private void ResetFilter(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox checkBox in _checkBoxes)
            {
                checkBox.IsChecked = false;
            }
            CollectionViewSource.GetDefaultView(_vm.CombinationModels).Refresh();
        }
    }
}
