using Cs4rsa.BaseClasses;

using System.Windows.Controls;
namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class SaveSessionUC : UserControl, IDialog
    {
        public SaveSessionUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return true;
        }
    }
}
