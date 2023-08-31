using CommunityToolkit.Mvvm.ComponentModel;

namespace Cs4rsa.BaseClasses
{
    public abstract partial class DialogVmBase : ViewModelBase
    {
        [ObservableProperty]
        private bool _isDialogOpen;

        [ObservableProperty]
        private bool _isCloseOnClickAway;

        [ObservableProperty]
        private IDialog _dialogUc;

        protected DialogVmBase()
        {
            IsDialogOpen = false;
            IsCloseOnClickAway = false;
        }

        protected override void OpenDialog(IDialog uc)
        {
            if (uc == null) return;
            DialogUc = uc;
            IsDialogOpen = true;
            IsCloseOnClickAway = uc.IsCloseOnClickAway();
        }

        protected override void CloseDialog()
        {
            IsDialogOpen = false;
        }
    }
}
