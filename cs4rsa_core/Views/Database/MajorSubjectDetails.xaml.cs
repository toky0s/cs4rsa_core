using Cs4rsa.Cs4rsaDatabase.DTOs;

using MaterialDesignThemes.Wpf.Transitions;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Cs4rsa.Views.Database
{
    public partial class MajorSubjectDetails
    {
        public MajorSubjectDetails()
        {
            InitializeComponent();
        }

        private void Btn_GotoMajor_Clicked(object sender, RoutedEventArgs e)
        {
            Transitioner.MoveFirstCommand.Execute(null, (Button)sender);
        }

        private void BtnViewCache_Clicked(object sender, RoutedEventArgs e)
        {
            DtoDbProgramSubject dtoDbProgramSubject = (DtoDbProgramSubject)((Hyperlink)sender).DataContext;
            OpenHtmlCacheFile(dtoDbProgramSubject.CourseId);
        }
    }
}
