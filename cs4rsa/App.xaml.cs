using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using cs4rsa.Database;
using System.Net.NetworkInformation;
using System.Net;
using cs4rsa.Dialogs.MessageBoxService;
using cs4rsa.Helpers;
using System.IO;
using Newtonsoft.Json;
using cs4rsa.Settings;
using System.Windows.Threading;

namespace cs4rsa
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConnectInternetChecker.Check();
            Cs4rsaData cs4RsaData = new Cs4rsaData();
            SettingsWriter.GetInstance().InitSettings();
         }
    }
}
