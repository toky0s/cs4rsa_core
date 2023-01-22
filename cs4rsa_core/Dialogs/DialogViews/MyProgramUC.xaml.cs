using Cs4rsa.BaseClasses;

using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class MyProgramUC : UserControl, IDialog
    {
        public MyProgramUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway() => true;
    }
}
