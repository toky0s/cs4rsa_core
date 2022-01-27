using cs4rsa_core.BaseClasses;
using System.Windows.Controls;

namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class ShareStringUC : UserControl, IDialog
    {
        public ShareStringUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }
    }
}
