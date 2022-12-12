using Cs4rsa.Models;
using Cs4rsa.ViewModels;
using Cs4rsa.ViewModels.AutoScheduling;

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

namespace Cs4rsa.Views.AutoScheduling
{
    public partial class ProgramTree : UserControl
    {
        public ProgramTree()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (DataContext as ProgramTreeViewModel).SelectedProSubject = e.NewValue is ProgramSubjectModel ? e.NewValue as ProgramSubjectModel : null;
        }

        // Chống scroll auto đưa item vào trung tâm khi focus
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
