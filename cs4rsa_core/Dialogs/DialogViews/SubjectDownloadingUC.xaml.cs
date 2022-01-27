using cs4rsa_core.BaseClasses;
using System.Windows.Controls;

namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class SubjectDownloadingUC : UserControl, IDialog
    {
        public SubjectDownloadingUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return false;
        }
    }
}
