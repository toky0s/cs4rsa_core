using Cs4rsa.BaseClasses;

using System.Threading.Tasks;

namespace Cs4rsa.ViewModels.ManualScheduling
{
    internal partial class MainSchedulingViewModel : ViewModelBase, IScreenViewModel
    {
        public MainSchedulingViewModel()
        {
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
