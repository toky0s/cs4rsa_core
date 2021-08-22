using cs4rsa.Enums;

namespace cs4rsa.Models.Base
{
    /// <summary>
    /// Là base của ProgramFolderModel và ProgramSubjectModel.
    /// </summary>
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
