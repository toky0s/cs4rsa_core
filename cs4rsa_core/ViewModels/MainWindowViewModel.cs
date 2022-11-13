using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using cs4rsa_core.BaseClasses;
using cs4rsa_core.Constants;
using cs4rsa_core.Dialogs.DialogViews;
using cs4rsa_core.Messages.Publishers;

using MaterialDesignThemes.Wpf;

namespace cs4rsa_core.ViewModels
{
    /// <summary>
    /// MainWindowViewModel này đảm nhiệm phẩn xử lý điều hướng và hiển thị thông báo
    /// trong các View. 
    /// Thực hiện khai báo các dịch vụ triển khai DI. 
    /// Thực hiện các chức năng liên quan đến đóng mở Dialog.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        #region Bindings
        private string _appName;
        public string AppName
        {
            get { return _appName; }
            set { _appName = value; OnPropertyChanged(); }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; OnPropertyChanged(); }
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        private bool _isOpen;
        public bool IsOpenDialog
        {
            get { return _isOpen; }
            set { _isOpen = value; OnPropertyChanged(); }
        }

        private object _dialogUC;
        public object DialogUC
        {
            get { return _dialogUC; }
            set { _dialogUC = value; OnPropertyChanged(); }
        }

        private bool _isCloseOnClickAway;
        public bool IsCloseOnClickAway
        {
            get { return _isCloseOnClickAway; }
            set { _isCloseOnClickAway = value; OnPropertyChanged(); }
        }

        private ISnackbarMessageQueue _snackBarMessageQueue;
        public ISnackbarMessageQueue SnackbarMessageQueue
        {
            get { return _snackBarMessageQueue; }
            set { _snackBarMessageQueue = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public RelayCommand OpenUpdateWindowCommand { get; set; }
        #endregion

        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            _snackBarMessageQueue = snackbarMessageQueue;
            _snackBarMessageQueue.Enqueue(ViewConstants.WELCOME_TEXT);

            WeakReferenceMessenger.Default.Register<HomeVmMsgs.UpdateSubjectDbMsg>(this, (r, m) =>
            {
                OnOpenUpdateWindow();
            });

            OpenUpdateWindowCommand = new RelayCommand(OnOpenUpdateWindow);

            SelectedIndex = 0;
            IsExpanded = false;
            AppName = ViewConstants.APP_NAME;
        }

        private void OnOpenUpdateWindow()
        {
            UpdateUC updateUC = new();
            OpenModal(updateUC);
        }

        public void OpenModal(IDialog uc)
        {
            if (uc != null)
            {
                DialogUC = uc;
            }
            IsOpenDialog = true;
            IsCloseOnClickAway = uc.IsCloseOnClickAway();
        }

        public void CloseModal() => IsOpenDialog = false;
    }
}
