using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs4rsa.Models.Base
{
    /// <summary>
    /// Là base của ProgramFolderModel và ProgramSubjectModel.
    /// </summary>
    public class TreeItem
    {
        public string Name { get; set; }
        public string Id { get; set; }

        public TreeItem(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
