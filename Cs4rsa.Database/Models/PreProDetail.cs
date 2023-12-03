namespace Cs4rsa.Database.Models
{
    /// <summary>
    /// Một môn có nhiều môn tiên quyết.
    /// </summary>
    public class PreProDetail
    {
        public int ProgramSubjectId { get; set; }
        public DbProgramSubject ProgramSubject { get; set; }
        public int PreParSubjectId { get; set; }
        public DbPreParSubject PreParSubject { get; set; }
    }
}
