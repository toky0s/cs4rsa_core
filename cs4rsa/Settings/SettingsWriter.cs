using cs4rsa.Crawler;
using Newtonsoft.Json;
using System.IO;

namespace cs4rsa.Settings
{
    /// <summary>
    /// Đảm nhiệm việc ghi setting vào cs4rsa_settings.json, nhận vào một Cs4rsaSetting.
    /// </summary>
    public class SettingsWriter
    {
        public static readonly string SettingsFileName = "cs4rsa_settings.json";
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
            File.WriteAllText(SettingsFileName, jsonString);
        }

        public void InitSettings()
        {
            if (!File.Exists(SettingsFileName))
            {
                string jsonString = JsonConvert.SerializeObject(_defaultSettings);
                File.WriteAllText(SettingsFileName, jsonString);
            }
        }
    }
}
