using ProgramSubjectCrawlerService.DataTypes.Enums;

namespace ProgramSubjectCrawlerService.DataTypes.Interfaces
{
    /// <summary>
    /// IProgramNode đại diện cho một node trong chương trình học của một
    /// sinh viên. Các Folder Node hay Subject Node đều phải kế thừa từ
    /// Interface này.
    /// 
    /// Tác giả: Xin
    /// Ngày thêm mới: ???
    /// Phiên bản: 0.0.2
    /// </summary>
    public interface IProgramNode
    {
        /// <summary>
        /// Lấy ra ID của một node
        /// </summary>
        /// <returns>ID node</returns>
        string GetIdNode();

        /// <summary>
        /// Lấy ra node con của node này
        /// </summary>
        /// <returns>Mã lớp con</returns>
        string GetChildOfNode();

        /// <summary>
        /// Lấy ra kiểu node
        /// </summary>
        /// <returns>Node Type</returns>
        NodeType GetNodeType();
    }
}
