using cs4rsa.Database;
using System.Configuration;

namespace cs4rsa.Settings
{
    public class SettingWriter
    {
        /// <summary>
        /// Sửa giá trị của một setting.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int EditSetting(Setting setting, string value)
        {
            Cs4rsaDatabase cs4RsaDatabase = Cs4rsaDatabase.GetInstance();
            string sql = $@"UPDATE user_settings
                            SET value = '{value}'
                            WHERE key = '{setting}'";
            return cs4RsaDatabase.DoSomething(sql);
        }
    }
}
