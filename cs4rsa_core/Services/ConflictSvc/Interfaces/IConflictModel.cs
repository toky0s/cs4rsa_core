using Cs4rsa.Services.ConflictSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;

namespace Cs4rsa.Services.ConflictSvc.Interfaces
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
