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
        private UpdateInfo _updateInfo;
        private UpdateManager _manager;
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
                // Tải bản cập nhật
                await _manager.DownloadUpdatesAsync(_updateInfo, updateProgress =>
                {
                    // Cập nhật tiến trình tải xuống (nếu cần)
                    _logger.LogInformation($"Downloading update: {updateProgress}%");
                    UpdateProgress = updateProgress;
                }, token);
                // Thông báo cho người dùng và restart để áp dụng
                _logger.LogInformation("Download completed. Applying updates and restarting application...");
                _manager.ApplyUpdatesAndRestart(_updateInfo);
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
        public DownloadUpdatesDialogViewModel(ILogger<DownloadUpdatesDialogViewModel> logger)
        {
            _logger = logger;
        }

        public string Title => "Download Updates";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog()
        {
            return false;
        }

        public void OnDialogClosed()
        {
            _logger.LogInformation("Download updates dialog closed");
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _updateInfo = parameters.GetValue<Velopack.UpdateInfo>("NewVersion");
            _manager = parameters.GetValue<Velopack.UpdateManager>("Manager");
            UpdateCommand.Execute();
        }
    }
}
