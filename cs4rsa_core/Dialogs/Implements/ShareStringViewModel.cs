using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogServices;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Windows;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ShareStringViewModel : DialogViewModelBase<ShareStringResult>
    {
        private string _shareString;
        public string ShareString
        {
            get { return _shareString ?? "Không có Share String nào ở đây cả."; }
            set
            {
                _shareString = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand CopyCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }

        public Action CloseDialogCallback { get; set; }

        #region Service
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public ShareStringViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue;

            CopyCommand = new RelayCommand(OnCopy, CanCopy);
            CloseDialogCommand = new RelayCommand(OnCloseDialog);
        }

        private void OnCloseDialog()
        {
            CloseDialogCallback.Invoke();
        }

        private bool CanCopy()
        {
            return _shareString != "Không có Share String nào ở đây cả.";
        }

        private void OnCopy()
        {
            if (_shareString != null)
            {
                Clipboard.SetData(DataFormats.Text, _shareString);
                string message = "Đã sao chép ShareString vào Clipboard";
                _snackbarMessageQueue.Enqueue(message);
            }
        }
    }
}
