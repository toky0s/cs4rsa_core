using Cs4rsa.BaseClasses;

using System.Windows.Controls;
namespace Cs4rsa.Dialogs.DialogViews
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
