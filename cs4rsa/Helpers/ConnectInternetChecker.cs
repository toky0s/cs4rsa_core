using cs4rsa.Dialogs.MessageBoxService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// Lớp kiểm tra kết nối internet.
    /// </summary>
    class ConnectInternetChecker
    {
        private static bool IsConnectingToInternet()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                Ping ping = new Ping();
                IPStatus status = ping.Send(new IPAddress(new byte[] { 8, 8, 8, 8 }), 2000).Status;
                if (status == IPStatus.Success)
                    return true;
            }
            return false;
        }

        public static void Check()
        {
            if (!IsConnectingToInternet())
            {
                throw new Exception("ket noi internet");
            }
        }
    }
}
