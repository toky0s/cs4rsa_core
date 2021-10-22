using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace cs4rsa_core.Settings
{
    public class Cs4rsaSetting
    {
        public string CurrentYearValue { get; set; }
        public string CurrentSemesterValue { get; set; }
        public string IsDatabaseCreated { get; set; }
        public string Version { get; set; }
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}
