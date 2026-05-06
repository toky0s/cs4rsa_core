using NuGet.Versioning;

using System;
using System.Threading;
using System.Threading.Tasks;

using Velopack;

namespace Cs4rsa.App.Services
{
    internal class DummyUpdateService : IUpdateService
    {
        public Task<UpdateInfo> HasNewVersion()
        {
            var asset = new VelopackAsset
            {
                PackageId = "Cs4rsa",
                Version = new NuGetVersion(9, 9, 9),
                Type = VelopackAssetType.Full,
                FileName = "Cs4rsa-9.9.9-full.nupkg",
                Size = 1024 * 1024 * 50, // 50MB
            };

            var updateInfo = new UpdateInfo(asset, false);
            return Task.FromResult(updateInfo);
        }

        public async Task UpdateNewVersion(UpdateInfo newVersion, Action<int> updateProgress, CancellationToken token)
        {
            const int totalSteps = 100;
            const int delayPerStep = 50; // ms -> tổng ~5 giây

            for (int i = 0; i <= totalSteps; i++)
            {
                token.ThrowIfCancellationRequested();
                updateProgress?.Invoke(i);
                await Task.Delay(delayPerStep, token);
            }

            Environment.Exit(0);
        }
    }
}