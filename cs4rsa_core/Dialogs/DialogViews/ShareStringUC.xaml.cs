using Cs4rsa.BaseClasses;

using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
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
