using Cs4rsa.BaseClasses;

using System.Windows.Controls;
namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class ProSubjectDetailUC : UserControl, IDialog
    {
        public ProSubjectDetailUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return false;
        }
    }
}
