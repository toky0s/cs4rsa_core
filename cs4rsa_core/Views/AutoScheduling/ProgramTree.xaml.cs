using Cs4rsa.Models;
using Cs4rsa.ViewModels.AutoScheduling;

using System.Windows;
using System.Windows.Controls;

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
            (DataContext as ProgramTreeViewModel).SltSubjectTreeItem = e.NewValue is ProgramSubjectModel
                                                                        ? e.NewValue as ProgramSubjectModel
                                                                        : null;
        }

        // Chống scroll auto đưa item vào trung tâm khi focus
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
