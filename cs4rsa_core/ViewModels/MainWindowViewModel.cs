using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Interfaces;

using MaterialDesignThemes.Wpf;

using System;

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
        private readonly IUnitOfWork _unitOfWork;

        #region Bindings
        [ObservableProperty]
        private int _storedScreenIdx;
        
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

        public MainWindowViewModel(
             ISnackbarMessageQueue snackbarMessageQueue
           , IUnitOfWork unitOfWork)
        {
            _snackBarMessageQueue = snackbarMessageQueue;
            _unitOfWork = unitOfWork;
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

        internal void SaveScreenIdx(string currentMenuItemIndex)
        {
            _unitOfWork.Settings.InsertOrUpdateLastOfScreenIndex(currentMenuItemIndex);
        }

        internal void LoadInfor()
        {
            bool succeeded = int.TryParse(_unitOfWork.Settings.GetBykey(VmConstants.StLastOfScreenIdx), out int @result);
            StoredScreenIdx = succeeded ? result : 0;
        }
    }
}
