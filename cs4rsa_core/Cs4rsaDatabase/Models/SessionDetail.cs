using System.Collections.Generic;

namespace cs4rsa_core.Cs4rsaDatabase.Models
{
    public class SessionDetail
    {
        public int SessionDetailId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string ClassGroup { get; set; }
        public string RegisterCode { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public List<SessionSchoolClass> SessionSchoolClasses { get; set; }
    }
}
