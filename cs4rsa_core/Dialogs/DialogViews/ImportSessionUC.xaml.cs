using Cs4rsa.BaseClasses;
using Cs4rsa.Dialogs.DialogResults;
using Cs4rsa.Dialogs.Implements;
using Cs4rsa.ViewModels.ManualScheduling;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Cs4rsa.Dialogs.DialogViews
{
    public partial class ImportSessionUC : BaseUserControl, IDialog
    {
        public ImportSessionUC() : base()
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ImportSessionViewModel)DataContext).OnCopyRegisterCode(((UserSubject)((Button)sender).DataContext).RegisterCode);
        }

        /// <summary>
        /// Xử lý sự kiện Share string được focus.
        /// </summary>
        private void ShareString_GotFocus(object sender, RoutedEventArgs e)
        {
            (DataContext as ImportSessionViewModel).LoadShareString(ShareStringTextBox.Text);
            (DataContext as ImportSessionViewModel).SelectedScheduleSession = null;
        }

        private void ShareString_TextChanged(object sender, TextChangedEventArgs e)
        {
            (DataContext as ImportSessionViewModel).LoadShareString(ShareStringTextBox.Text);
            (DataContext as ImportSessionViewModel).SelectedScheduleSession = null;
        }

        private void TgBtn_IsUseCache_Loaded(object sender, RoutedEventArgs e)
        {
            ((ToggleButton)sender).DataContext = Container.GetService(typeof(SearchViewModel));
        }
    }
}
