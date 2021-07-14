using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using System.Windows;
using LightMessageBus;
using cs4rsa.Messages;

namespace cs4rsa.Dialogs.Implements
{
    public class ShareStringViewModel : DialogViewModelBase<ShareStringResult>
    {
        private string _shareString;
        public string ShareString
        {
            get
            {
                return _shareString;
            }
            set
            {
                _shareString = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand CopyCommand { get; set; }

        public ShareStringViewModel(string shareString)
        {
            if (shareString == null)
                _shareString = "Không có Share String nào ở đây cả.";
            else
                _shareString = shareString;
            CopyCommand = new RelayCommand(OnCopy, CanCopy);
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
