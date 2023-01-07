using Cs4rsa.BaseClasses;

using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
{
    /// <summary>
    /// Interaction logic for AutoFilterUC.xaml
    /// </summary>
    public partial class AutoFilterUC : UserControl, IDialog
    {
        public AutoFilterUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }
    }
}
