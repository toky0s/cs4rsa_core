using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Dialogs.Implements;

using System.Windows.Controls;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class ImportSessionUC : UserControl, IDialog
    {
        public ImportSessionUC()
        {
            InitializeComponent();
        }
        public bool IsCloseOnClickAway()
        {
            return true;
        }

        /// <summary>
        /// Xử lý sự kiện Người dùng Click nút sao chép mã đăng ký.
        /// </summary>
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as ImportSessionViewModel).OnCopyRegisterCode(((UserSubject)((Button)sender).DataContext).RegisterCode);
        }

        /// <summary>
        /// Xử lý sự kiện Share string được focus.
        /// </summary>
        private void ShareString_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as ImportSessionViewModel).LoadShareString(ShareStringTextBox.Text);
            (DataContext as ImportSessionViewModel).SelectedScheduleSession = null;
        }

        private void ShareString_TextChanged(object sender, TextChangedEventArgs e)
        {
            (DataContext as ImportSessionViewModel).LoadShareString(ShareStringTextBox.Text);
            (DataContext as ImportSessionViewModel).SelectedScheduleSession = null;
        }
    }
}
