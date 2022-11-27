using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Settings.Interfaces;

namespace Cs4rsa.ViewModels
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

        private readonly ISetting _setting;

        public InfoViewModel(ISetting setting)
        {
            _setting = setting;

            Version = _setting.Read(VMConstants.STPROPS_VERSION);
            IsChecking = false;
        }
    }
}
