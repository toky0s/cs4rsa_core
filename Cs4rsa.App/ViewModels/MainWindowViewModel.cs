using Cs4rsa.App.Events.TopMenuEvents;
using Cs4rsa.App.Views.UserControls;
using Cs4rsa.Infrastructure.Events;
using Cs4rsa.Service.Dialog.Events;
using Cs4rsa.Service.Notification;
using Cs4rsa.Service.Notification.Models;

using MaterialDesignThemes.Wpf;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Velopack;
using Velopack.Sources;

namespace Cs4rsa.App.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<MainWindowViewModel> _logger;
        private readonly IDialogService _dialogService;

        private DelegateCommand _checkForUpdatesCommand;
        public DelegateCommand CheckForUpdatesCommand =>
            _checkForUpdatesCommand ?? (_checkForUpdatesCommand = new DelegateCommand(async () => await ExecuteCheckForUpdatesCommand(), CanExecuteCheckForUpdatesCommand));

        async Task ExecuteCheckForUpdatesCommand()
        {
            var source = Properties.Resources.GithubSource;
            var mgr = new UpdateManager(new GithubSource(source, null, false));

            UpdateInfo newVersion = null;
            try
            {
                // Kiểm tra xem có bản mới không
                newVersion = await mgr.CheckForUpdatesAsync();
            }
            catch (Velopack.Exceptions.NotInstalledException)
            {
                _logger.LogTrace("No previous version installed or you haven't installed the app yet, treating as new version available.");
                MessageBox.Show("You are in latest version", "Check for Updates", MessageBoxButton.OK);
                return;
            }
            if (newVersion == null)
            {
                MessageBox.Show("You are in latest version", "Check for Updates", MessageBoxButton.OK);
            }
            else
            {
                var message = $"New version available: {newVersion.BaseRelease.Version}, this will exit your app immediately, apply updates. Do you want to update?";
                var result = MessageBox.Show(message, "Check for Updates", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _logger.LogInformation($"User agreed to update to version {newVersion.TargetFullRelease.Version}. Starting download...");
                    var parameters = new DialogParameters
                    {
                        { "NewVersion", newVersion },
                        { "Manager", mgr }
                    };
                    _dialogService.ShowDialog(nameof(DownloadUpdatesDialog), parameters, null);
                }
            }
        }

        bool CanExecuteCheckForUpdatesCommand()
        {
            return true;
        }

        #region Notification Service Region
        public ObservableCollection<Notification> NotificationItems { get; set; }
        #endregion

        public MainWindowViewModel(
            IEventAggregator eventAggregator,
            ILogger<MainWindowViewModel> logger,
            IDialogService dialogService)
        {
            NotificationItems = new ObservableCollection<Notification>();
            _eventAggregator = eventAggregator;
            _logger = logger;
            _dialogService = dialogService;

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
