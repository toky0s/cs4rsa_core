using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BaseClasses;

namespace cs4rsa.ViewModels
{
    public class InfoViewModel: ViewModelBase
    {
        public string Version { get; set; }
        public InfoViewModel()
        {
            //Version = ConfigurationManager.AppSettings.Get("Version");
        }
    }
}
