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
using cs4rsa.ViewModels;

namespace cs4rsa.Views
{
    /// <summary>
    /// Interaction logic for ClassGroupSession.xaml
    /// </summary>
    public partial class ClassGroupSession : UserControl
    {
        private ClassGroupViewModel classGroupViewModel = new ClassGroupViewModel();
        public ClassGroupSession()
        {
            InitializeComponent();
            this.DataContext = classGroupViewModel;
        }
    }
}
