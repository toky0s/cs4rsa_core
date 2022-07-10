using cs4rsa_core.ViewModels;

using System;
using System.Windows;
using System.Windows.Controls;

namespace cs4rsa_core.Views
{
    public partial class StudentSearch : UserControl
    {
        public StudentSearch()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            (DataContext as StudentSearchViewModel).GetStudentImages();
        }
    }
}
