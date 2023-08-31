using Cs4rsa.ViewModels.ManualScheduling;

using System.Windows.Controls;

namespace Cs4rsa.Views.ManualScheduling
{
    public partial class Chose : UserControl
    {
        private object Vm;

        public Chose()
        {
            InitializeComponent();
        }

        private void Btn_SolveConflict_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((ChoseViewModel)DataContext).SolveConflictCommand.Execute(null);
        }

        private void Listbox_Choiced_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            ((ListBox)sender).ScrollIntoView(e.AddedItems[0]);
        }

        private void ContextMenu_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ContextMenu ctxMenu = (ContextMenu)sender;
            ctxMenu.DataContext = Vm;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Vm = DataContext;
        }
    }
}
