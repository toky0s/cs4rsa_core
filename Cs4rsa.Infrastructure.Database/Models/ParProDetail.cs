namespace Cs4rsa.Database.Models
{
    /// <summary>
    /// Một môn học có nhiều môn song hành.
    /// </summary>
    public class ParProDetail
    {
        public int ProgramSubjectId { get; set; }
        public DbProgramSubject ProgramSubject { get; set; }
        public int PreParSubjectId { get; set; }
        public DbPreParSubject PreParSubject { get; set; }
    }
}
