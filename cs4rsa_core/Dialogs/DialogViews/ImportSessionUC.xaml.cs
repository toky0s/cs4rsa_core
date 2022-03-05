using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.Implements;
using System.Windows.Controls;

namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class ImportSessionUC : UserControl, IDialog
    {
        public ImportSessionUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return true;
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (DataContext as ImportSessionViewModel).OnCopyRegisterCodeAtCurrentClassGroup();
        }
    }
}
