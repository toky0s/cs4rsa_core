using cs4rsa_core.BaseClasses;

using System.Windows.Controls;
namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class SessionInputUC : UserControl, IDialog
    {
        public SessionInputUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return false;
        }
    }
}
