
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;
using cs4rsa.Models.Base;

namespace cs4rsa.Models
{
    /// <summary>
    /// Là một node trong cây thư mục môn học.
    /// </summary>
    public class ProgramFolderModel: TreeItem
    {
        public ObservableCollection<ProgramFolderModel> ChildFolders;
        public ObservableCollection<ProgramSubjectModel> ChildSubjects;
        public ObservableCollection<TreeItem> ChildItems { get; set; }

        private string _folderName;
        public string FolderName
        {
            get { return _folderName; }
            set { _folderName = value; }
        }

        public ProgramFolderModel(ProgramFolder programFolder):base(programFolder.Name, programFolder.Id)
        {
            List<ProgramFolderModel> folders = programFolder.ChildProgramFolders
                .Select(item => new ProgramFolderModel(item))
                .ToList();
            ChildFolders = new ObservableCollection<ProgramFolderModel>(folders);

            List<ProgramSubjectModel> subjects = programFolder.ChildProgramSubjects
                .Select(item => new ProgramSubjectModel(item))
                .ToList();
            ChildSubjects = new ObservableCollection<ProgramSubjectModel>(subjects);

            ChildItems = new ObservableCollection<TreeItem>();

            foreach (TreeItem item in folders)
            {
                ChildItems.Add(item);
            }

            foreach (TreeItem item in subjects)
            {
                ChildItems.Add(item);
            }

            _folderName = programFolder.Name;
        }
    }
}
