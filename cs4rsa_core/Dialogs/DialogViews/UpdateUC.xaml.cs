using cs4rsa_core.BaseClasses;

using System.Windows.Controls;

namespace cs4rsa_core.Dialogs.DialogViews
{
    public partial class UpdateUC : UserControl, IDialog
    {
        public UpdateUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }
    }
}
