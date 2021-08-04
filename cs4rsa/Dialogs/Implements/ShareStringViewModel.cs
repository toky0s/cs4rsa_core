using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Messages;
using LightMessageBus;
using System;
using System.Windows;

namespace cs4rsa.Dialogs.Implements
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

        public ShareStringViewModel()
        {
            CopyCommand = new RelayCommand(OnCopy, CanCopy);
            CloseDialogCommand = new RelayCommand(OnCloseDialog);
        }

        private void OnCloseDialog(object obj)
        {
            CloseDialogCallback.Invoke();
        }

        private bool CanCopy()
        {
            return _shareString != "Không có Share String nào ở đây cả.";
        }

        private void OnCopy(object obj)
        {
            if (_shareString != null)
            {
                Clipboard.SetData(DataFormats.Text, _shareString);
                string message = "Đã sao chép ShareString vào Clipboard";
                MessageBus.Default.Publish<Cs4rsaSnackbarMessage>(new Cs4rsaSnackbarMessage(message));
            }
        }
    }
}
