using Cs4rsa.Constants;
using Cs4rsa.Services.CourseSearchSvc.Crawlers.Interfaces;
using Cs4rsa.Settings.Interfaces;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.IO;

namespace Cs4rsa.Settings
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
            Cs4rsaSetting defaultSettings = new()
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
                using StreamReader file = File.OpenText(SettingsFileName);
                using JsonTextReader reader = new(file);
                JObject settingJson = (JObject)JToken.ReadFrom(reader);
                CurrentSetting = settingJson.ToObject<Cs4rsaSetting>();
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
                throw new Exception(VMConstants.EX_SETTING_DOES_NOT_EXIST);
            }
        }

        public void Save()
        {
            string jsonString = JsonConvert.SerializeObject(CurrentSetting);
            File.WriteAllText(SettingsFileName, jsonString);
        }
    }
}
