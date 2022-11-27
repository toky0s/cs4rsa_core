namespace Cs4rsa.Cs4rsaDatabase.Models
{
    /// <summary>
    /// Một môn có nhiều môn tiên quyết.
    /// </summary>
    public class PreProDetail
    {
        public int ProgramSubjectId { get; set; }
        public ProgramSubject ProgramSubject { get; set; }
        public int PreParSubjectId { get; set; }
        public PreParSubject PreParSubject { get; set; }
    }
}
