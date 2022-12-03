using Cs4rsa.ViewModels;

using System;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views
{
    public partial class Search : UserControl
    {
        public Search()
        {
            InitializeComponent();
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
            await (DataContext as SearchViewModel).LoadSearchItemSource(text);
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
                (DataContext as SearchViewModel).OnAddSubjectFromUriAsync(uri);
            }
        }
    }
}