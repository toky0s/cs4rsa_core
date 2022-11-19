using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.ViewModels;

using System;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views
{
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
            await searchViewModel.LoadSavedSchedules();
        }

        private async void SearchingTextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string text = (sender as TextBox).Text;
            if (text.Trim().Length > 0)
            {
                Popup_Recommend.IsOpen = true;
            }
            else
            {
                Popup_Recommend.IsOpen = false;
            }
            await (DataContext as SearchSessionViewModel).LoadSearchItemSource(text);
        }

        private void SearchingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Popup_Recommend.IsOpen = false;
        }

        private void DownloadSubjects_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                string url = (string)e.Data.GetData(DataFormats.UnicodeText);
                Uri uri = new UriBuilder(url).Uri;
                (DataContext as SearchSessionViewModel).OnAddSubjectFromUriAsync(uri);
            }
        }
    }
}