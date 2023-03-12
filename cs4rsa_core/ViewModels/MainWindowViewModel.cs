using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.BaseClasses;

using MaterialDesignThemes.Wpf;

namespace Cs4rsa.ViewModels
{
    /// <summary>
    /// MainWindowViewModel này đảm nhiệm phần xử lý điều hướng và hiển thị thông báo
    /// trong các View. 
    /// Thực hiện khai báo các dịch vụ triển khai DI. 
    /// Thực hiện các chức năng liên quan đến đóng mở Dialog.
    /// </summary>
    public partial class MainWindowViewModel : ObservableRecipient
    {
        #region Bindings
        [ObservableProperty]
        private bool _isExpanded;

        [ObservableProperty]
        private bool _isOpen;

        [ObservableProperty]
        private bool _isWindowEnable;

        [ObservableProperty]
        private object _dialogUC;

        [ObservableProperty]
        private bool _isCloseOnClickAway;

        [ObservableProperty]
        private ISnackbarMessageQueue _snackBarMessageQueue;
        #endregion

        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackBarMessageQueue = snackbarMessageQueue;
            IsExpanded = false;
            IsWindowEnable = true;
        }

        public void OpenModal(IDialog uc)
        {
            if (uc != null)
            {
                DialogUC = uc;
            }
            IsOpen = true;
            IsCloseOnClickAway = uc.IsCloseOnClickAway();
        }

        public void CloseModal() => IsOpen = false;
    }
}
