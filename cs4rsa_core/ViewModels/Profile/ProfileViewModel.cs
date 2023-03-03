using Cs4rsa.BaseClasses;

using System.Threading.Tasks;

namespace Cs4rsa.ViewModels.Profile
{
    internal partial class ProfileViewModel : ViewModelBase, IScreenViewModel
    {
        public void InitData()
        {

        }

        public Task InitDataAsync()
        {
            return Task.FromResult<object>(null);
        }
    }
}
