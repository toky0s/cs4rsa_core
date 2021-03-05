using System.Windows;
using cs4rsa.ViewModels;

namespace cs4rsa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SemesterInfo_Loaded(object sender, RoutedEventArgs e)
        {
            SemesterInfoViewModel semesterInfoViewModel = new SemesterInfoViewModel();
            semesterInfoViewModel.LoadSemesterInfo();
            SemesterInfo.DataContext = semesterInfoViewModel;
        }
    }
}
