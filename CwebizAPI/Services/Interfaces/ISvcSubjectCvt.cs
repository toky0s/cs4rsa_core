using CwebizAPI.Crawlers.SubjectCrawlerSvc.DataTypes;
using CwebizAPI.DTOs.Responses;

namespace CwebizAPI.Services.Interfaces;

public interface ISvcSubjectCvt
{
    Task<DtoRpSubject> ToDtoRpSubject(Subject subject);
    Task<DtoRpClassGroup> ToDtoRpClassGroup(ClassGroup classGroup, bool isBelongSpecialSubject, string color);
    Task<DtoRpSchoolClass> ToDtoRpSchoolClass(SchoolClass schoolClass, string color);
}