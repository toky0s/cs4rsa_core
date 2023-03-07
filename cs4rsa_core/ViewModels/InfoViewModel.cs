using Cs4rsa.BaseClasses;
using Cs4rsa.Constants;
using Cs4rsa.Settings.Interfaces;

using System.Threading.Tasks;

namespace Cs4rsa.ViewModels
{
    public class InfoViewModel : ViewModelBase, IScreenViewModel
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

            Version = _setting.Read(VmConstants.StVersion);
            IsChecking = false;
        }

        public void InitData()
        {

        }

        public Task InitDataAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
