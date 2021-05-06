using System.Windows;
using cs4rsa.ViewModels;
using System.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using cs4rsa.Helpers;

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
            SemesterInfo.DataContext = semesterInfoViewModel;
        }
    }
}
