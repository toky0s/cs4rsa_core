using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.Implements;

using System.Windows.Controls;
using System.Windows.Input;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class SaveSessionUC : UserControl
    {
        public SaveSessionUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Lưu bộ lịch khi người dùng nhấn Enter
        /// </summary>
        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            ICommand saveCommand = (DataContext as SaveSessionViewModel).SaveCommand;
            if (e.Key == Key.Enter && saveCommand.CanExecute(this))
            {
                (DataContext as SaveSessionViewModel).SaveCommand.Execute(this);
            }
        }
    }
}
