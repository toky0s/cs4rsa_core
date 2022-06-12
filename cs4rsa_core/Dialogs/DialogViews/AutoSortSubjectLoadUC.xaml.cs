using cs4rsa_core.BaseClasses;

using System.Windows.Controls;
namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class AutoSortSubjectLoadUC : UserControl, IDialog
    {
        public AutoSortSubjectLoadUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return false;
        }
    }
}
