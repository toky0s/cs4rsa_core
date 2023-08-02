using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes;
using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes.Enums;
using CwebizAPI.Crawlers.TeacherCrawlerSvc.Models;

namespace CwebizAPI.DTOs.Responses;

public class DtoRpSchoolClass
{
    public string Color { get; set; }
    public string SubjectCode { get; set; }
    public string SchoolClassName { get; set; }
    public string SubjectName { get; set; }
    public string RegisterCode { get; set; }
    public string EmptySeat { get; set; }
    public string RegistrationTermEnd { get; set; }
    public string RegistrationTermStart { get; set; }
    public string RegistrationStatus { get; set; }
    public string ImplementationStatus { get; set; }
    public ClassForm Type { get; set; }
    public DayPlaceMetaData DayPlaceMetaData { get; set; }
    public Schedule Schedule { get; set; }
    public StudyWeek StudyWeek { get; set; }
    public IEnumerable<string> Rooms { get; set; }
    public IEnumerable<string> TempTeachers { get; set; }
    public IEnumerable<Place> Places { get; set; }
    public Phase Phase { get; set; }
}