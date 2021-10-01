﻿using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Helpers;
using cs4rsa.Implements;
using cs4rsa.Interfaces;
using cs4rsa.Messages;
using LightMessageBus;
using LightMessageBus.Interfaces;
using MaterialDesignThemes.Wpf;
using System;
using System.Configuration;

namespace cs4rsa.ViewModels
{

    /// <summary>
    /// MainViewModel này đảm nhiệm phẩn xử lý điều hướng và hiển thị thông báo
    /// trong các View. Thực hiện khai báo các dịch vụ triển khai DI. Thực hiện
    /// các chức năng liên quan đến đóng mở Dialog.
    /// </summary>
    public class MainViewModel : ViewModelBase, IMessageHandler<Cs4rsaSnackbarMessage>
    {
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

        private SnackbarMessageQueue _snackBarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(2000));
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get { return _snackBarMessageQueue; }
            set { _snackBarMessageQueue = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            MessageBus.Default.FromAny().Where<Cs4rsaSnackbarMessage>().Notify(this);
            _snackBarMessageQueue.Enqueue("Chào mừng đến với CS4RSA");
        }

        public void Handle(Cs4rsaSnackbarMessage message)
        {
            _snackBarMessageQueue.Enqueue(message.Source);
        }


        /// <summary>
        /// Hiển thị một UserControl dưới dạng một Dialog.
        /// </summary>
        /// <param name="uc">UserControl</param>
        public void OpenDialog(object uc)
        {
            if (uc != null)
            {
                DialogUC = uc;
            }
            IsOpenDialog = true;
        }

        /// <summary>
        /// Đóng Dialog.
        /// </summary>
        public void CloseDialog() => IsOpenDialog = false;
    }
}
