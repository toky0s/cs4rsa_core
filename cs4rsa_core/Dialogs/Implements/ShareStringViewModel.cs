using cs4rsa_core.Dialogs.DialogResults;
using cs4rsa_core.Dialogs.DialogServices;
using MaterialDesignThemes.Wpf;
using Microsoft.Toolkit.Mvvm.Input;
using System.Windows;

namespace cs4rsa_core.Dialogs.Implements
{
    public class ShareStringViewModel : DialogViewModelBase<ShareStringResult>
    {
        private static readonly string _defaultShareString = "Không có Share String nào ở đây cả.";

        #region Bindings
        private string _shareString;
        public string ShareString
        {
            get
            {
                if (_shareString == string.Empty)
                {
                    return _defaultShareString;
                }
                return _shareString;
            }
            set
            {
                _shareString = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Commands
        public RelayCommand CopyCommand { get; set; }
        public RelayCommand CloseDialogCommand { get; set; }
        #endregion

        #region Service
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public ShareStringViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackbarMessageQueue = snackbarMessageQueue;

            CopyCommand = new RelayCommand(OnCopy, CanCopy);
            CloseDialogCommand = new RelayCommand(CloseD);
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
