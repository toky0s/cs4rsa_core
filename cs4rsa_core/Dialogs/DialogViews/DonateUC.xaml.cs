using Cs4rsa.BaseClasses;

using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class DonateUC : UserControl, IDialog
    {
        public DonateUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }
    }
}
