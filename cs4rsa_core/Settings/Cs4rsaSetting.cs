namespace Cs4rsa.Settings
{
    public class Cs4rsaSetting
    {
        public string CurrentYear { get; set; }
        public string CurrentSemester { get; set; }
        public string CurrentYearValue { get; set; }
        public string CurrentSemesterValue { get; set; }
        public string IsDatabaseCreated { get; set; }
        public string Version { get; set; }
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
    }
}
