using Cs4rsa.App.Services;

using Microsoft.Extensions.Logging;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Threading;
using System.Threading.Tasks;

using Velopack;

namespace Cs4rsa.App.ViewModels
{
    public class DownloadUpdatesDialogViewModel : BindableBase, IDialogAware
    {
        private readonly ILogger<DownloadUpdatesDialogViewModel> _logger;
        private readonly IUpdateService _updateService;

        private UpdateInfo _updateInfo;
        private bool _isDowloading;

        private DelegateCommand _exitDownloadingUpdate;
        public DelegateCommand ExitDownloadingUpdate =>
            _exitDownloadingUpdate ?? (_exitDownloadingUpdate = new DelegateCommand(ExecuteExitDownloadingUpdate, CanExecuteExitDownloadingUpdate));

        void ExecuteExitDownloadingUpdate()
        {
            _logger.LogInformation("User request exit downloading update");
            CancellationTokenSource.Cancel();
#if DEBUG
            _isDowloading = false;
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
#endif
        }

        bool CanExecuteExitDownloadingUpdate()
        {
#if DEBUG
            return true;
#else
            return _isDowloading;
#endif
        }

        private DelegateCommand _updateCommand;
        public DelegateCommand UpdateCommand =>
            _updateCommand ?? (_updateCommand = new DelegateCommand(async () => await ExecuteUpdateCommand(), CanExecuteUpdateCommand));

        private int _updateProgress;
        public int UpdateProgress
        {
            get { return _updateProgress; }
            set { SetProperty(ref _updateProgress, value); }
        }

        private CancellationTokenSource _cancellationTokenSource;
        public CancellationTokenSource CancellationTokenSource => _cancellationTokenSource ?? (_cancellationTokenSource = new CancellationTokenSource());

        async Task ExecuteUpdateCommand()
        {
            var token = CancellationTokenSource.Token;
            try
            {
                _isDowloading = true;
                ExitDownloadingUpdate.RaiseCanExecuteChanged();
                await _updateService.UpdateNewVersion(_updateInfo, updateProgress =>
                {
                    // Cập nhật tiến trình tải xuống (nếu cần)
                    _logger.LogInformation($"Downloading update: {updateProgress}%");
                    UpdateProgress = updateProgress;
                }, token);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning("Update cancelled by user.", ex.Message);
                _isDowloading = false;
#if DEBUG
                //RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
#endif
            }
#if DEBUG
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug("Request canceled.", ex);
            }
#endif
        }

        bool CanExecuteUpdateCommand()
        {
            return true;
        }

        public DownloadUpdatesDialogViewModel(
            ILogger<DownloadUpdatesDialogViewModel> logger,
            IUpdateService updateService)
        {
            _logger = logger;
            _updateService = updateService;
        }

        public string Title => "Download Updates";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
#if DEBUG
            return true;
#else
            return !_isDowloading;
#endif
        }

        public void OnDialogClosed()
        {
#if DEBUG
            CancellationTokenSource.Cancel();
#else
            _logger.LogInformation("Download updates dialog closed");
#endif
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _isDowloading = false;
            _updateInfo = parameters.GetValue<UpdateInfo>("NewVersion");
            UpdateCommand.Execute();
        }

        private DelegateCommand _stopDownloadingCommand;
        public DelegateCommand StopDownloadingCommand =>
            _stopDownloadingCommand ?? (_stopDownloadingCommand = new DelegateCommand(ExecuteStopDownloadingCommand, CanExecuteStopDownloadingCommand));

        void ExecuteStopDownloadingCommand()
        {
            CancellationTokenSource.Cancel();
        }

        bool CanExecuteStopDownloadingCommand()
        {
            return true;
        }
    }
}
