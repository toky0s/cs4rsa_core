using System.Windows.Controls;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Views
{
    public partial class ShowDetailsSubjectUC : UserControl
    {
        public ShowDetailsSubjectUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            });
            e.Handled = true;
        }
    }
}
