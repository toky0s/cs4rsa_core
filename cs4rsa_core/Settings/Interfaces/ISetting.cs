namespace cs4rsa_core.Settings.Interfaces
{
    public interface ISetting
    {
        Cs4rsaSetting CurrentSetting { get; set; }
        void Save();
        string Read(string key);
    }
}
