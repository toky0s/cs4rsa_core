using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.Settings.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace cs4rsa_core.Settings
{
    public class Setting : ISetting
    {
        public Cs4rsaSetting CurrentSetting { get; set; }
        public string SettingsFileName { get; set; } = "cs4rsa_settings.json";

        private readonly ICourseCrawler _courseCrawler;
        public Setting(ICourseCrawler courseCrawler)
        {
            _courseCrawler = courseCrawler;
            Init();
        }

        public void Init()
        {
            Cs4rsaSetting defaultSettings = new Cs4rsaSetting
            {
                CurrentSemesterValue = _courseCrawler.GetCurrentSemesterValue(),
                CurrentYearValue = _courseCrawler.GetCurrentYearValue(),
                IsDatabaseCreated = "false",
                Version = "1.1.2"
            };
            if (!File.Exists(SettingsFileName))
            {
                string jsonString = JsonConvert.SerializeObject(defaultSettings);
                File.WriteAllText(SettingsFileName, jsonString);
                CurrentSetting = defaultSettings;
            }
            else
            {
                using (StreamReader file = File.OpenText(SettingsFileName))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject settingJson = (JObject)JToken.ReadFrom(reader);
                    CurrentSetting = settingJson.ToObject<Cs4rsaSetting>();
                }
            }
        }

        public string Read(string key)
        {
            try
            {
                return CurrentSetting[key].ToString();
            }
            catch
            {
                Exception exception = new("Setting is not exist");
                throw exception;
            }
        }

        public void Save()
        {
            string jsonString = JsonConvert.SerializeObject(CurrentSetting);
            File.WriteAllText(SettingsFileName, jsonString);
        }
    }
}
