using System.Windows;
using Cs4rsa.Service.Dialog.Interfaces;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;

namespace Cs4rsa.Module.ManuallySchedule.Dialogs.ViewModels
{
    public class ShareStringViewModel : BindableBase
    {
        private const string DefaultShareString = "Không có Share String nào ở đây cả.";

        #region Bindings
        private string _shareString;
        public string ShareString
        {
            get => _shareString == string.Empty ? DefaultShareString : _shareString;
            set => SetProperty(ref _shareString, value);
        }
        #endregion

        #region Commands
        public DelegateCommand CopyCommand { get; set; }
        public DelegateCommand CloseDialogCommand { get; set; }
        #endregion

        #region Service
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        #endregion

        public ShareStringViewModel(
            ISnackbarMessageQueue snackbarMessageQueue,
            IDialogService dialogService
        )
        {
            _snackbarMessageQueue = snackbarMessageQueue;

            CopyCommand = new DelegateCommand(OnCopy, CanCopy);
            CloseDialogCommand = new DelegateCommand(dialogService.CloseDialog);
        }

        private bool CanCopy()
        {
            return _shareString != "Không có Share String nào ở đây cả.";
        }

        private void OnCopy()
        {
            if (_shareString == null) return;
            Clipboard.SetData(DataFormats.Text, _shareString);
            _snackbarMessageQueue.Enqueue("Đã sao chép ShareString vào Clipboard");
        }
    }
}
