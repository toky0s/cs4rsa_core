using cs4rsa_core.BaseClasses;
using System.Windows.Controls;

namespace cs4rsa_core.Dialogs.DialogViews
{
    /// <summary>
    /// Interaction logic for UpdateUC.xaml
    /// </summary>
    public partial class UpdateUC : UserControl, IDialog
    {
        public UpdateUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return false;
        }
    }
}
