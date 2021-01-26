using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using cs4rsa.Crawler;

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
            HomeCourseSearch h = new HomeCourseSearch();
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\" + "cs4rsa_disciplines.json";
            string gesturefile = System.IO.Path.Combine(Environment.CurrentDirectory, path);
            h.DisciplineDatasToJsonFile(gesturefile);
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void enterLabel(object sender, MouseEventArgs e)
        {
            var labelTitle = sender as TextBlock;
            labelTitle.Text = "Truong A Xin";
        }
    }
}
