using CommunityToolkit.Mvvm.ComponentModel;

using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;

namespace Cs4rsa.Models.Bases
{
    public abstract class TreeItem : ObservableObject
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public NodeType NodeType { get; set; }

        public TreeItem(string name, string id) : base()
        {
            Name = name;
            Id = id;
        }
    }
}
