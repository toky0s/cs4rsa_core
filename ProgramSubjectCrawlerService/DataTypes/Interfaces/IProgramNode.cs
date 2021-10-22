using ProgramSubjectCrawlerService.DataTypes.Enums;

namespace ProgramSubjectCrawlerService.DataTypes.Interfaces
{
    public interface IProgramNode
    {
        string GetIdNode();
        string GetChildOfNode();
        NodeType GetNodeType();
    }
}
