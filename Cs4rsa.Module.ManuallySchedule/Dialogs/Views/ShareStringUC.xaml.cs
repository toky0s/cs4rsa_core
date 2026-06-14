using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;

using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Views
{
    public partial class ShareStringUC : UserControl
    {
        public ShareStringUC()
        {
            InitializeComponent();
        }

        private async void CopyButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((ShareStringUCViewModel)DataContext).CopyCommand.Execute();
            CopyButton.Content = "Copied!";
            await Task.Delay(3000);
            CopyButton.Content = "Copy";
        }
    }
}
