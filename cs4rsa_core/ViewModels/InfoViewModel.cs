using cs4rsa_core.BaseClasses;
using cs4rsa_core.Interfaces;
using cs4rsa_core.Settings.Interfaces;

using FirebaseService.Interfaces;

using MaterialDesignThemes.Wpf;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Threading.Tasks;

namespace cs4rsa_core.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        public string Version { get; set; }

        private bool _isChecking;
        public bool IsChecking
        {
            get { return _isChecking; }
            set { _isChecking = value; OnPropertyChanged(); }
        }

        public AsyncRelayCommand CheckUpdateCommand { get; set; }

        private readonly ISetting _setting;
        private readonly IFirebase _firebase;
        private readonly ISnackbarMessageQueue _snackbarMessageQueue;
        private readonly IOpenInBrowser _openInBrowser;
        public InfoViewModel(ISetting setting, IFirebase firebase,
            ISnackbarMessageQueue snackbarMessageQueue, IOpenInBrowser openInBrowser)
        {
            _setting = setting;
            _firebase = firebase;
            _snackbarMessageQueue = snackbarMessageQueue;
            _openInBrowser = openInBrowser;

            CheckUpdateCommand = new(OnCheckUpdate);

            Version = _setting.Read("Version");
            IsChecking = false;
        }

        private async Task OnCheckUpdate()
        {
            IsChecking = true;
            string strLatestVersion = await _firebase.GetLatestVersion();
            Version localVersion = new(Version);
            if (strLatestVersion != null)
            {
                Version latestVersion = new(strLatestVersion);
                int result = localVersion.CompareTo(latestVersion);
                if (result < 0)
                {
                    string message = $"Phiên bản {latestVersion} đã có mặt";
                    _snackbarMessageQueue.Enqueue(message, "TẢI XUỐNG", OnGotoInstanceSource);
                }
                else if (result == 0)
                {
                    string message = $"Đang là phiên bản mới nhất";
                    _snackbarMessageQueue.Enqueue(message);
                }
            }
            IsChecking = false;
        }

        private void OnGotoInstanceSource()
        {
            string url = "https://drive.google.com/drive/folders/1mtnhC8AmVsPO0KnyOueQRbvcyHVnMzxO?usp=sharing";
            _openInBrowser.Open(url);
        }
    }
}
