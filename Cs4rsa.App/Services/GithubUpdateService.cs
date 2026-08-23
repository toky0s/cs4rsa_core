using Microsoft.Extensions.Logging;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Velopack;
using Velopack.Sources;

namespace Cs4rsa.App.Services
{
    internal class GithubUpdateService : IUpdateService
    {
        private readonly ILogger<GithubUpdateService> _logger;
        private readonly UpdateManager updateManager;
        public GithubUpdateService(ILogger<GithubUpdateService> logger)
        {
            var source = Properties.Resources.GithubSource;
            updateManager = new UpdateManager(new GithubSource(source, null, false));
            _logger = logger;
        }

        public async Task<UpdateInfo> HasNewVersion()
        {
            UpdateInfo newVersion = null;
            try
            {
                // Kiểm tra xem có bản mới không
                newVersion = await updateManager.CheckForUpdatesAsync();
            }
            catch (Velopack.Exceptions.NotInstalledException)
            {
                _logger.LogTrace("No previous version installed or you haven't installed the app yet, treating as new version available.");
                MessageBox.Show("You are in latest version", "Check for Updates", MessageBoxButton.OK);   
            }
            return newVersion;
        }

        public async Task UpdateNewVersion(UpdateInfo newVersion, Action<int> updateProgress, CancellationToken token)
        {
            // Tải bản cập nhật
            await updateManager.DownloadUpdatesAsync(newVersion, updateProgress, token);
            // Thông báo cho người dùng và restart để áp dụng
            _logger.LogInformation("Download completed. Applying updates and restarting application...");
            updateManager.ApplyUpdatesAndRestart(newVersion);
        }
    }
}
