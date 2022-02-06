using cs4rsa_core.BaseClasses;
using cs4rsa_core.Dialogs.DialogServices;
using cs4rsa_core.Messages;
using LightMessageBus;
using LightMessageBus.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace cs4rsa_core.ViewModels
{

    /// <summary>
    /// MainWindowViewModel này đảm nhiệm phẩn xử lý điều hướng và hiển thị thông báo
    /// trong các View. Thực hiện khai báo các dịch vụ triển khai DI. Thực hiện
    /// các chức năng liên quan đến đóng mở Dialog.
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
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

        private SnackbarMessageQueue _snackBarMessageQueue;
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get { return _snackBarMessageQueue; }
            set { _snackBarMessageQueue = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            SnackbarMessageQueue = (SnackbarMessageQueue)snackbarMessageQueue;
            _snackBarMessageQueue.Enqueue("Chào mừng đến với CS4RSA");

            SelectedIndex = 0;
        }

        public void OpenDialog(IDialog uc)
        {
            if (uc != null)
            {
                DialogUC = uc;
            }
            IsOpenDialog = true;
            IsCloseOnClickAway = uc.IsCloseOnClickAway();
        }

        public void CloseDialog() => IsOpenDialog = false;
    }
}
