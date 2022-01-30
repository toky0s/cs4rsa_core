namespace Cs4rsaDatabaseService.Models
{
    public class SessionSchoolClass
    {
        public int SessionSchoolClassId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public SessionDetail SessionDetail { get; set; }
    }
}
