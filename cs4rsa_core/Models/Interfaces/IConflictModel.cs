
using ConflictService.DataTypes.Enums;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;

namespace cs4rsa_core.Models.Interfaces
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
