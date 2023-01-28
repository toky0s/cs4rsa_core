using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.ViewModels.Database;

using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views.Database
{
    public partial class Db : UserControl
    {
        public Db()
        {
            InitializeComponent();
        }

        private void WbCacheContent_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Uri != null)
            {
                e.Cancel = true;
            }
        }

        private async void BtnViewCache_Clicked(object sender, RoutedEventArgs e)
        {
            Keyword kw = (Keyword)((Button)sender).DataContext;
            await ((DbViewModel)DataContext).ViewCacheCommand.ExecuteAsync(kw.CourseId);
        }
    }
}
