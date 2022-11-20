using Cs4rsa.BaseClasses;

using System.Windows.Controls;
namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class SubjectImporterUC : UserControl, IDialog
    {
        public SubjectImporterUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return false;
        }
    }
}
