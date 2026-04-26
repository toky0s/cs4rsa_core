using Cs4rsa.App.Events.TopMenuEvents;
using Cs4rsa.Infrastructure.Events;
using Cs4rsa.Service.Dialog.Events;
using Cs4rsa.Service.Notification;
using Cs4rsa.Service.Notification.Models;
using MaterialDesignThemes.Wpf;

using Prism.Events;
using Prism.Mvvm;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.App.ViewModels
{
    public class MainWindowViewModel: BindableBase
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


        #region Notification Service Region
        public ObservableCollection<Notification> NotificationItems { get; set; }
        #endregion

        public MainWindowViewModel(
            IEventAggregator eventAggregator, 
            ISnackbarMessageQueue snackbarMessageQueue)
        {
            NotificationItems = new ObservableCollection<Notification>();
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<NotificationEvent>().Subscribe(args => {
                NotificationItems.Insert(0, new Notification
                {
                    Title = args.Title,
                    Content = args.Message,
                    CreatedOn = args.CreatedOn,
                    FromAction = args.FromAction,
                });
            });

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
