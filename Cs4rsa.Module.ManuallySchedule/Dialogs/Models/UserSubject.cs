namespace Cs4rsa.Module.ManuallySchedule.Dialogs.Models
{
    /// <summary>
    /// Là một item trong kết quả trả về của View User Schedule
    /// </summary>
    public class UserSubject
    {
        public string SubjectCode { get; set; }
        public string ClassGroup { get; set; }
        public string SubjectName { get; set; }
        public string RegisterCode { get; set; }
        public string SchoolClass { get; set; }
        /// <summary>
        /// Can be OK or NOT OK
        /// OK: It is available in this semester, and the user can add it to schedule.
        /// NOT OK: It is not available in this semester, and the user cannot add it to schedule.
        /// </summary>
        public string Status { get; set; }
    }
}
