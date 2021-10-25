using System;
using System.Net;
using System.Net.NetworkInformation;

namespace HelperService
{
    /// <summary>
    /// Lớp kiểm tra kết nối internet.
    /// </summary>
    public class ConnectInternetChecker
    {
        public static bool IsConnectingToInternet()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                Ping ping = new Ping();
                IPStatus status = ping.Send(new IPAddress(new byte[] { 8, 8, 8, 8 }), 2000).Status;
                if (status == IPStatus.Success)
                {
                    return true;
                }
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
