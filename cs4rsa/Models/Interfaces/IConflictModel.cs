using cs4rsa.BasicData;
using cs4rsa.Models.Enums;

namespace cs4rsa.Models.Interfaces
{
    /// <summary>
    /// Triển khai ConflictModel hiển thị các ConflictPlace hoặc ConflictTime
    /// </summary>
    public interface IConflictModel
    {
        ClassGroup FirstClassGroup { get; set; }
        ClassGroup SecondClassGroup { get; set; }
        string GetConflictInfo();
        ConflictType GetConflictType();
        Phase GetPhase();
    }
}
