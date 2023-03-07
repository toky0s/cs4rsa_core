using Cs4rsa.ViewModels.ManualScheduling;

using System.Windows.Controls;

namespace Cs4rsa.Views.ManualScheduling
{
    public partial class Choosed : UserControl
    {
        public Choosed()
        {
            InitializeComponent();
        }

        private void Btn_SolveConflict_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((ChoosedViewModel)DataContext).SolveConflictCommand.Execute(null);
        }
    }
}
