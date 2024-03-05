using Cs4rsa.App.Events.TopMenuEvents;
using Cs4rsa.Infrastructure.Events;
using Cs4rsa.Service.Dialog.Events;

using MaterialDesignThemes.Wpf;

using Prism.Events;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.App.ViewModels
{
    internal class MainWindowViewModel: BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private ISnackbarMessageQueue _snackBarMessageQueue;
        public ISnackbarMessageQueue SnackBarMessageQueue
        {
            get { return _snackBarMessageQueue; }
            set { SetProperty(ref _snackBarMessageQueue, value); }
        }

        private int _screenIdx;
        public int ScreenIdx
        {
            get { return _screenIdx; }
            set { SetProperty(ref _screenIdx, value); }
        }

        private object _dialog;
        public object Dialog
        {
            get { return _dialog; }
            set { SetProperty(ref _dialog, value); }
        }

        private bool _isCloseOnClickAway;
        public bool IsCloseOnClickAway
        {
            get { return _isCloseOnClickAway; }
            set { SetProperty(ref _isCloseOnClickAway, value); }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetProperty(ref _isOpen, value); }
        }

        public MainWindowViewModel(
            IEventAggregator eventAggregator, 
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            _eventAggregator = eventAggregator;
            SnackBarMessageQueue = snackbarMessageQueue;
            var hs = snackbarMessageQueue.GetHashCode();

            _eventAggregator.GetEvent<ScreenChangedEvent>().Subscribe((idx) => ScreenIdx = idx);
            _eventAggregator.GetEvent<CloseDialogEvent>().Subscribe(() =>
            {
                IsOpen = false;
                Dialog = null;
                IsCloseOnClickAway = true;
            });
            _eventAggregator.GetEvent<OpenDialogEvent>().Subscribe(payload =>
            {
                IsOpen = true;
                Dialog = payload.Item1;
                IsCloseOnClickAway = payload.Item2;
            });

            _eventAggregator.GetEvent<SnackbarMsgEvent>().Subscribe(msg =>
            {
                SnackBarMessageQueue.Enqueue(msg);
            });
        }
    }
}
