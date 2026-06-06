using Cs4rsa.Module.Shared;

using Prism.Commands;
using Prism.Mvvm;

namespace Cs4rsa.App.ViewModels
{
    public class StatusWindowViewModel : BindableBase
    {
        private bool _connected;
        public bool Connected
        {
            get { return _connected; }
            set { SetProperty(ref _connected, value); }
        } 
    }
}
