using ProgramSubjectCrawlerService.DataTypes.Enums;
namespace cs4rsa_core.Models.Bases
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
