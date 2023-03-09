using Cs4rsa.BaseClasses;
using Cs4rsa.Models;
using Cs4rsa.ViewModels.AutoScheduling;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Cs4rsa.Views.AutoScheduling
{
    public partial class ProgramTree : UserControl, IComponent
    {
        private static readonly int PROGRAM_TREE_INDEX = 0;
        private static readonly int FILTER_INDEX = 1;

        public ProgramTree()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Chống scroll auto đưa item vào trung tâm khi focus
        /// </summary>
        private void TreeViewItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            ProgramTreeViewModel proTreeVm = (DataContext as ProgramTreeViewModel);
            Button btn = (Button)sender;
            ProgramSubjectModel psm = (ProgramSubjectModel)btn.DataContext;
            proTreeVm.AddCommand.Execute(psm);
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            ProgramTreeViewModel ProTreeVm = (DataContext as ProgramTreeViewModel);
            Button btn = (Button)sender;
            ProgramSubjectModel psm = (ProgramSubjectModel)btn.DataContext;
            ProTreeVm.DeleteCommand.Execute(psm);
        }

        private void GotoFilterBtn_Click(object sender, RoutedEventArgs e)
        {
            Trans1.SelectedIndex = Trans1.SelectedIndex == PROGRAM_TREE_INDEX ? FILTER_INDEX : PROGRAM_TREE_INDEX;
        }

        public async Task LoadData()
        {
            await ((ProgramTreeViewModel)DataContext).LoadStudents();
        }
    }
}
