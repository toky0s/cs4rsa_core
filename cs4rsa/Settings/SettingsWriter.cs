using cs4rsa.Crawler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Settings
{
    public class SettingsWriter
    {
        private static readonly string _settingsFileName = "cs4rsa_settings.json";
        private static SettingsWriter _instance = new SettingsWriter();

        private static HomeCourseSearch _homeCourseSearch = HomeCourseSearch.GetInstance();

        private static Cs4rsaSetting _defaultSettings = new Cs4rsaSetting
        {
            CurrentSemesterValue = _homeCourseSearch.CurrentSemesterValue,
            CurrentYearValue = _homeCourseSearch.CurrentYearValue
        };

        private SettingsWriter()
        {

        }

        public static SettingsWriter GetInstance()
        {
            return _instance;
        }

        public void Save(Cs4rsaSetting cs4RsaSetting)
        {
            string jsonString = JsonConvert.SerializeObject(cs4RsaSetting);
            File.WriteAllText(_settingsFileName, jsonString);
        }

        public void InitSettings()
        {
            if (!File.Exists(_settingsFileName))
            {
                string jsonString = JsonConvert.SerializeObject(_defaultSettings);
                File.WriteAllText(_settingsFileName, jsonString);
            }
        }
    }
}
