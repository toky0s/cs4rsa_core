using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Cs4rsa.Module.Shared
{
    public class NetworkMonitor
    {
        private bool _isCurrentConnected;
        public bool IsCurrentConnected { get => _isCurrentConnected; }
        public event Action<bool> ConnectivityChanged;

        public NetworkMonitor()
        {
            NetworkChange.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += OnNetworkAddressChanged;
        }

        private async void OnNetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            await CheckInternetAsync();
        }

        private async void OnNetworkAddressChanged(object sender, EventArgs e)
        {
            await CheckInternetAsync();
        }

        public void Dispose()
        {
            NetworkChange.NetworkAvailabilityChanged -= OnNetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged -= OnNetworkAddressChanged;
        }

        public async Task CheckInternetAsync()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync("8.8.8.8", timeout: 3000);
                    _isCurrentConnected = reply.Status == IPStatus.Success;
                    var success = reply.Status == IPStatus.Success;
                    ConnectivityChanged?.Invoke(success);
                }
            }
            catch
            {
                _isCurrentConnected = false;
                ConnectivityChanged?.Invoke(false);
            }
        }
    }
}
