using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;
using LightMessageBus.Interfaces;
using cs4rsa.Messages;
using LightMessageBus;

namespace cs4rsa.ViewModels
{
    /// <summary>
    /// MainViewModel này đảm nhiệm phẩn xử lý điều hướng và hiển thị thông báo
    /// trong các View.
    /// </summary>
    public class MainViewModel: NotifyPropertyChangedBase, IMessageHandler<Cs4rsaSnackbarMessage>
    {
        private SnackbarMessageQueue _snackBarMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3000));
        public SnackbarMessageQueue SnackbarMessageQueue
        {
            get { return _snackBarMessageQueue; }
            set { _snackBarMessageQueue = value; RaisePropertyChanged(); }
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
    }
}
