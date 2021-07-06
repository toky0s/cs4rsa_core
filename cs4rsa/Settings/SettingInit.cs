using System.Configuration;
using System;
using System.Collections.Generic;
using cs4rsa.Database;
using cs4rsa.Enums;

namespace cs4rsa.Settings
{
    /// <summary>
    /// Thêm tất cả setting cần thiết vào lần khởi tạo đầu tiên.
    /// </summary>
    public class SettingInit
    {
        private static Dictionary<Setting, string> _settingDict = new Dictionary<Setting, string>()
        {
            {Setting.IsDynamicSchedule, "1"},
            {Setting.IsShowPlaceColor, "0" }
        };
        
        public SettingInit()
        {
            foreach (KeyValuePair<Setting, string> item in _settingDict)
            {
                AddSetting(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Thêm mới một setting vào database. Nếu setting tồn tại trả về 
        /// false, nếu chưa tồn tại trả về true.
        /// </summary>
        /// <param name="key">Tên Setting từ lớp Setting.</param>
        /// <param name="value">Giá trị của setting.</param>
        /// <returns></returns>
        private static bool AddSetting(Setting key, string value)
        {
            if (SettingReader.IsExistsSetting(key))
                return false;
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $@"insert into user_settings values ('{key}', '{value}')";
            cs4RsaDatabase.DoSomething(sql);
            return true;
        }
    }
}
