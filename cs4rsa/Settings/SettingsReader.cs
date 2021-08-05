using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cs4rsa.Settings
{
    public class SettingsReader
    {
        private JObject _settingJson;
        public Cs4rsaSetting Setting => _settingJson.ToObject<Cs4rsaSetting>();

        public SettingsReader()
        {
            string fileName = SettingsWriter.SettingsFileName;
            using (StreamReader file = File.OpenText(fileName))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                _settingJson = (JObject)JToken.ReadFrom(reader);
            }
        }
    }
}
