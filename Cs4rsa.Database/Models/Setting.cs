using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cs4rsa.Database.Models
{
    public class Setting
    {
        public const string SemesterValue = "CurrentSemesterValue";
        public const string YearValue = "CurrentYearValue";
        public const string SemesterInfo = "CurrentSemesterInfo";
        public const string YearInfo = "CurrentYearInfo";

        private readonly Dictionary<string, string> _settings;

        public Setting()
        {
            _settings = new Dictionary<string, string>();
        }

        public void AddSetting(string key, string value)
        {
            bool exists = _settings.TryGetValue(key, out _);
            if (exists)
            {
                _settings[key] = value;
            }
            else
            {
                _settings.Add(key, value);
            }
        }

        public string GetValue(string key)
        {
            return _settings[key];
        }
    }
}
