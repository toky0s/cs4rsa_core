using Cs4rsa.BaseClasses;

using System.Windows.Controls;
namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class SolveConflictUC : UserControl, IDialog
    {
        public SolveConflictUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return true;
        }
    }
}
