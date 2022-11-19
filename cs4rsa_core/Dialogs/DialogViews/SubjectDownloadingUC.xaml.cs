using Cs4rsa.BaseClasses;

using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
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
