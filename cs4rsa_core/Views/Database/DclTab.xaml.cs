using Cs4rsa.Cs4rsaDatabase.Models;

using System.Windows;
using System.Windows.Documents;

namespace Cs4rsa.Views.Database
{
    public partial class DclTab
    {
        public DclTab()
        {
            InitializeComponent();
        }

        private void BtnViewCache_Clicked(object sender, RoutedEventArgs e)
        {
            Keyword kw = (Keyword)((Hyperlink)sender).DataContext;
            OpenHtmlCacheFile(kw.CourseId.ToString());
        }
    }
}
