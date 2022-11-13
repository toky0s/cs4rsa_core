using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.Implements;

using System.Windows.Controls;

namespace cs4rsa_core.Dialogs.DialogViews
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
