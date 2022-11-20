namespace Cs4rsa.Settings.Interfaces
{
    public interface ISetting
    {
        Cs4rsaSetting CurrentSetting { get; set; }
        void Save();
        string Read(string key);
    }
}
