using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.Implements;

using System.Windows.Controls;
using System.Windows.Input;

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
    }
}
