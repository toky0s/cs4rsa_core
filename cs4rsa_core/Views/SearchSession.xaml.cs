using cs4rsa_core.ViewModels;
using Cs4rsaDatabaseService.Models;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
{
    /// <summary>
    /// Interaction logic for SearchSession.xaml
    /// </summary>
    public partial class SearchSession : UserControl
    {
        public SearchSession()
        {
            InitializeComponent();
        }

        private void DisciplineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchSessionViewModel searchViewModel = DataContext as SearchSessionViewModel;
            if (searchViewModel.SelectedDiscipline != null)
            {
                searchViewModel.LoadDisciplineKeyword((Discipline)DisciplineComboBox.SelectedItem);
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            menu.DataContext = DataContext;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SearchSessionViewModel searchViewModel = DataContext as SearchSessionViewModel;
            await searchViewModel.LoadDiscipline();
        }
    }
}
