using Cs4rsa.App.Services;
using Cs4rsa.App.Views.UserControls;
using Cs4rsa.Database.Interfaces;
using Cs4rsa.Service.Notification;
using Cs4rsa.Service.Notification.Models;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Cs4rsa.App.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IDialogService _dialogService;
        private readonly IUpdateService _updateService;
        private readonly IUnitOfWork _unitOfWork;

        private DelegateCommand _checkForUpdatesCommand;
        public DelegateCommand CheckForUpdatesCommand =>
            _checkForUpdatesCommand ?? (_checkForUpdatesCommand = new DelegateCommand(async () => await ExecuteCheckForUpdatesCommand(), CanExecuteCheckForUpdatesCommand));

        async Task ExecuteCheckForUpdatesCommand()
        {
            var newVersion = await _updateService.HasNewVersion();
            if (newVersion == null)
            {
                MessageBox.Show("You are in latest version", "Check for Updates", MessageBoxButton.OK);
            }
            else
            {
                var message = $"New version available: {newVersion.TargetFullRelease.Version}, this will exit your app immediately, apply updates. Do you want to update?";
                var result = MessageBox.Show(message, "Check for Updates", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _logger.LogInformation($"User agreed to update to version {newVersion.TargetFullRelease.Version}. Starting download...");
                    var parameters = new DialogParameters
                    {
                        { "NewVersion", newVersion },
                    };
                    _dialogService.ShowDialog(nameof(DownloadUpdatesDialog), parameters, null);
                }
            }
        }

        bool CanExecuteCheckForUpdatesCommand()
        {
            return true;
        }

        private DelegateCommand _resetCacheCommand;
        public DelegateCommand ResetCacheCommand =>
            _resetCacheCommand ?? (_resetCacheCommand = new DelegateCommand(ExecuteResetCacheCommand, CanExecuteResetCacheCommand));

        void ExecuteResetCacheCommand()
        {
            _logger.LogInformation("User reset cache");
            var n = _unitOfWork.Keywords.ResetCache();
            _logger.LogInformation("Number of keywords reset: {Count}", n);
        }

        bool CanExecuteResetCacheCommand()
        {
            return true;
        }

        #region Notification Service Region
        public ObservableCollection<Notification> NotificationItems { get; set; }
        #endregion

        public MainWindowViewModel(
            IEventAggregator eventAggregator,
            ILogger<MainWindowViewModel> logger,
            IDialogService dialogService,
            IUpdateService updateService,
            IUnitOfWork unitOfWork)
        {
            NotificationItems = new ObservableCollection<Notification>();
            _eventAggregator = eventAggregator;
            _logger = logger;
            _dialogService = dialogService;
            _updateService = updateService;
            _unitOfWork = unitOfWork;

            _eventAggregator.GetEvent<NotificationEvent>().Subscribe(args =>
            {
                NotificationItems.Insert(0, new Notification
                {
                    Title = args.Title,
                    Content = args.Message,
                    CreatedOn = args.CreatedOn,
                    FromAction = args.FromAction,
                });
            });
        }
    }
}
