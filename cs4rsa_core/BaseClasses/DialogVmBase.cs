using CommunityToolkit.Mvvm.ComponentModel;

namespace Cs4rsa.BaseClasses
{
    internal abstract partial class DialogVmBase : ViewModelBase
    {
        [ObservableProperty]
        private bool _isDialogOpen;

        [ObservableProperty]
        private bool _isCloseOnClickAway;

        [ObservableProperty]
        private IDialog _dialogUC;

        public DialogVmBase() : base()
        {
            IsDialogOpen = false;
            IsCloseOnClickAway = false;
        }

        protected override void OpenDialog(IDialog uc)
        {
            if (uc != null)
            {
                DialogUC = uc;
            }
            IsDialogOpen = true;
            IsCloseOnClickAway = uc.IsCloseOnClickAway();
        }

        protected override void CloseDialog()
        {
            IsDialogOpen = false;
        }
    }
}
