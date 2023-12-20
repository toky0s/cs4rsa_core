using System.Windows.Controls;
using System.Windows.Input;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Views
{
    public partial class SaveSessionUC : UserControl
    {
        public SaveSessionUC()
        {
            InitializeComponent();
        }

        public bool IsCloseOnClickAway()
        {
            return true;
        }

        /// <summary>
        /// Lưu bộ lịch khi người dùng nhấn Enter
        /// </summary>
        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            ICommand saveCommand = (DataContext as SaveSessionViewModel).SaveCommand;
            if (e.Key == Key.Enter && saveCommand.CanExecute(this))
            {
                (DataContext as SaveSessionViewModel).SaveCommand.Execute();
            }
        }
    }
}
