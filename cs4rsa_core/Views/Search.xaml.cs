using Cs4rsa.ViewModels;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Views
{
    public partial class Search : UserControl
    {
        private static readonly Key[] _userAllowedKeys = { Key.OemMinus, Key.Back, Key.Space };
        public Search()
        {
            InitializeComponent();
        }

        public static bool IsKeyAChar(Key key)
        {
            return key >= Key.A && key <= Key.Z;
        }

        public static bool IsKeyADigit(Key key)
        {
            return (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
        }

        private static bool IsUserAllowedKey(Key key)
        {
            return _userAllowedKeys.Contains(key);
        }

        private void SearchingTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Popup_Recommend.IsOpen = (IsKeyAChar(e.Key) || IsKeyADigit(e.Key) || IsUserAllowedKey(e.Key))
                && e.Key != Key.Escape;
            if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
            }
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

        private void SearchingTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Popup_Recommend.IsOpen = SearchingTextBox.Text.Trim().Length > 0;
        }
    }
}