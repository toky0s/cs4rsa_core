using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Models.Bases;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Utils;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Cs4rsa.Models
{
    /// <summary>
    /// Là một node trong cây thư mục môn học.
    /// </summary>
    public class ProgramFolderModel : TreeItem
    {
        public ObservableCollection<ProgramFolderModel> ChildFolders { get; set; }
        public ObservableCollection<ProgramSubjectModel> ChildSubjects { get; set; }
        public ObservableCollection<TreeItem> ChildItems { get; set; }
        public string FolderName { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }

        private readonly ProgramFolder _programFolder;
        private readonly ColorGenerator _colorGenerator;
        private readonly IUnitOfWork _unitOfWork;
        public ProgramFolderModel(ProgramFolder programFolder, ColorGenerator colorGenerator, IUnitOfWork unitOfWork) : base(programFolder.Name, programFolder.Id)
        {
            _colorGenerator = colorGenerator;
            _unitOfWork = unitOfWork;
            _programFolder = programFolder;

            FolderName = programFolder.Name;
            Description = programFolder.Description;
            IsCompleted = programFolder.IsCompleted();
            NodeType = programFolder.GetNodeType();
        }

        private async Task<ProgramFolderModel> InitializeAsync(ProgramFolder programFolder)
        {
            List<ProgramFolderModel> folders = new();
            foreach (ProgramFolder item in programFolder.ChildProgramFolders)
            {
                ProgramFolderModel programFolderModel = await ProgramFolderModel.CreateAsync(item, _colorGenerator, _unitOfWork);
                folders.Add(programFolderModel);
            }
            ChildFolders = new ObservableCollection<ProgramFolderModel>(folders);

            ChildItems = new ObservableCollection<TreeItem>();
            foreach (TreeItem item in folders)
            {
                ChildItems.Add(item);
            }

            List<ProgramSubjectModel> subjects = new();
            foreach (ProgramSubject item in _programFolder.ChildProgramSubjects)
            {
                ProgramSubjectModel programSubjectModel = await ProgramSubjectModel.CreateAsync(item, _colorGenerator, _unitOfWork);
                subjects.Add(programSubjectModel);
            }
            ChildSubjects = new ObservableCollection<ProgramSubjectModel>(subjects);
            foreach (TreeItem item in subjects)
            {
                ChildItems.Add(item);
            }
            return this;
        }

        public static Task<ProgramFolderModel> CreateAsync(ProgramFolder programFolder, ColorGenerator colorGenerator, IUnitOfWork unitOfWork)
        {
            ProgramFolderModel ret = new(programFolder, colorGenerator, unitOfWork);
            return ret.InitializeAsync(programFolder);
        }
    }
}
