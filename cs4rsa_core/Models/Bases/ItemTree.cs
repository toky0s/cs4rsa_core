using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;

namespace Cs4rsa.Models.Bases
{
    public class TreeItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public NodeType NodeType { get; set; }

        public TreeItem(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
