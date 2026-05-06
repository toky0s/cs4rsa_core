using Cs4rsa.App.Services;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;

using Velopack;

namespace Cs4rsa.App.ViewModels
{
    public class DownloadUpdatesDialogViewModel : BindableBase, IDialogAware
    {
        private readonly ILogger<DownloadUpdatesDialogViewModel> _logger;
        private readonly IUpdateService _updateService;
        private UpdateInfo _updateInfo;

        private DelegateCommand _exitDownloadingUpdate;
        public DelegateCommand ExitDownloadingUpdate =>
            _exitDownloadingUpdate ?? (_exitDownloadingUpdate = new DelegateCommand(ExecuteExitDownloadingUpdate, CanExecuteExitDownloadingUpdate));

        void ExecuteExitDownloadingUpdate()
        {
            _logger.LogInformation("User request exit downloading update");
            CancellationTokenSource.Cancel();
            RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        bool CanExecuteExitDownloadingUpdate()
        {
            return true;
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
                await _updateService.UpdateNewVersion(_updateInfo, updateProgress =>
                {
                    // Cập nhật tiến trình tải xuống (nếu cần)
                    _logger.LogInformation($"Downloading update: {updateProgress}%");
                    UpdateProgress = updateProgress;
                }, token);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning("Update cancelled by user.", ex.Message);
            }
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
            return true;
        }

        public void OnDialogClosed()
        {
            _logger.LogInformation("Download updates dialog closed");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _updateInfo = parameters.GetValue<Velopack.UpdateInfo>("NewVersion");
            UpdateCommand.Execute();
        }
    }
}
