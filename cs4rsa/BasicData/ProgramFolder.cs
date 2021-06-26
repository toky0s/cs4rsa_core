using cs4rsa.Interfaces;
using System.Collections.Generic;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho hình thức học của một Folder, có thể là bắt buộc
    /// có thể là Chọn n trong k môn có trong folder.
    /// Compulisory: Bắt buộc
    /// AllowSelection: Cho phép chọn n trong k môn
    /// NonDefine: Không có định nghĩa về phần này
    /// </summary>
    public enum StudyMode
    {
        Compulsory,
        AllowSelection,
        NonDefine
    }

    public class ProgramFolder : IProgramNode, IComparer<ProgramFolder>
    {
        private string _id;
        private string _childOfNode;
        private string _name;
        private List<ProgramFolder> _childProgramFolders = new List<ProgramFolder>();
        private List<ProgramSubject> _childProgramSubjects = new List<ProgramSubject>();
        private string _rawHtml;
        private StudyMode _studyMode;

        public string Id => _id;
        public string ChildOfNode => _childOfNode;
        public string Name { get => _name; private set => _name = value; }
        public List<ProgramFolder> ChildProgramFolders => _childProgramFolders;
        public List<ProgramSubject> ChildProgramSubjects => _childProgramSubjects;
        public StudyMode StudyMode => _studyMode;
        public string RawHtml => _rawHtml;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Tên node.</param>
        /// <param name="studyMode">Bắt buộc, chọn n trong k hoặc không có.</param>
        /// <param name="isRoot">Là node gốc.</param>
        /// <param name="nodeId">Id node.</param>
        /// <param name="childOfNode">Id của node cha.</param>
        /// <param name="rawHtml">Html gốc.</param>
        public ProgramFolder(string name, StudyMode studyMode, string nodeId, string childOfNode, string rawHtml)
        {
            _id = nodeId;
            _childOfNode = childOfNode;
            _name = name;
            _studyMode = studyMode;
            _rawHtml = rawHtml;
        }


        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="folderNode"></param>
        public ProgramFolder(ProgramFolder folderNode)
        {
            _id = folderNode.Id;
            _childOfNode = folderNode.ChildOfNode;
            _name = folderNode.Name;
            _childProgramFolders = folderNode.ChildProgramFolders;
            _childProgramSubjects = folderNode.ChildProgramSubjects;
            _studyMode = folderNode.StudyMode;
            _rawHtml = folderNode.RawHtml;
        }

        /// <summary>
        /// Kiểm tra một mã môn có tồn tại trong folder này hay không.
        /// Nó sẽ đệ quy qua tất cả các folder con bên trong.
        /// </summary>
        /// <param name="subjectCode"></param>
        /// <returns></returns>
        public bool IsExistsSubject(string subjectCode)
        {
            return false;
        }

        public string GetIdNode()
        {
            return _id;
        }

        public string GetChildOfNode()
        {
            return _childOfNode;
        }

        public void AddNode(IProgramNode node)
        {
            if (node is ProgramSubject)
                _childProgramSubjects.Add(node as ProgramSubject);
            else
                _childProgramFolders.Add(node as ProgramFolder);
        }

        public void AddNodes(List<IProgramNode> nodes)
        {
            foreach (IProgramNode node in nodes)
            {
                AddNode(node);
            }
        }

        public void AddNodes(List<ProgramSubject> nodes)
        {
            _childProgramSubjects.AddRange(nodes);
        }

        public int Compare(ProgramFolder x, ProgramFolder y)
        {
            return x.ChildOfNode.CompareTo(y.ChildOfNode);
        }

        public override bool Equals(object obj)
        {
            ProgramFolder folder = obj as ProgramFolder;
            return _id.Equals(folder.Id);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
