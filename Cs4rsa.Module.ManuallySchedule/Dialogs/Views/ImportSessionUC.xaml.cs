using System.ComponentModel;
using Cs4rsa.Module.ManuallySchedule.Dialogs.Models;
using Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Cs4rsa.Module.ManuallySchedule.ViewModels;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Views
{
    public partial class ImportSessionUC
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
            ((ImportSessionUCViewModel)DataContext).OnCopyRegisterCode(((UserSubject)((Button)sender).DataContext).RegisterCode);
        }

        /// <summary>
        /// Xử lý sự kiện Share string được focus.
        /// </summary>
        private void ShareString_GotFocus(object sender, RoutedEventArgs e)
        {
            ((ImportSessionUCViewModel)DataContext).LoadShareString(ShareStringTextBox.Text);
            ((ImportSessionUCViewModel)DataContext).SelectedScheduleSession = null;
        }

        private void ShareString_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((ImportSessionUCViewModel)DataContext).LoadShareString(ShareStringTextBox.Text);
            ((ImportSessionUCViewModel)DataContext).SelectedScheduleSession = null;
        }

        private void TgBtn_IsUseCache_Loaded(object sender, RoutedEventArgs e)
        {
            // ((ToggleButton)sender).DataContext = Container.GetService(typeof(SearchViewModel));
        }
    }
}
