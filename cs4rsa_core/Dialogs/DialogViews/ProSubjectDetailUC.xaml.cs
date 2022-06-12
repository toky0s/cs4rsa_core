using cs4rsa_core.BaseClasses;

using System.Windows.Controls;
namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class ProSubjectDetailUC : UserControl, IDialog
    {
        public ProSubjectDetailUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return false;
        }
    }
}
