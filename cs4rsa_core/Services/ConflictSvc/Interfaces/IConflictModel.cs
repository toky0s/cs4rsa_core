using cs4rsa_core.Services.ConflictSvc.DataTypes.Enums;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes;
using cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes.Enums;

namespace cs4rsa_core.Services.ConflictSvc.Interfaces
{
    /// <summary>
    /// Triển khai ConflictModel hiển thị các ConflictPlace hoặc ConflictTime
    /// </summary>
    public interface IConflictModel
    {
        SchoolClass FirstSchoolClass { get; set; }
        SchoolClass SecondSchoolClass { get; set; }
        string GetConflictInfo();
        ConflictType GetConflictType();
        Phase GetPhase();
    }
}
