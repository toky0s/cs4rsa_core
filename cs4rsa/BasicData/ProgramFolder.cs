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

    public class ProgramFolder : IProgramNode
    {
        private string _id;
        private string _childOfNode;
        private string _name;
        private List<IProgramNode> _childProgramFolders;
        private List<IProgramNode> _programSubjects;
        private bool _isRoot;
        private string _rawHtml;
        private StudyMode _studyMode;

        public string Name { get => _name; private set => _name = value; }
        public List<IProgramNode> ChildProgramFolder { get => _childProgramFolders; private set => _childProgramFolders = value; }
        public List<IProgramNode> ProgramSubjects { get => _programSubjects; private set => _programSubjects = value; }
        public bool IsRoot { get => _isRoot; private set => _isRoot = value; }
        public StudyMode StudyMode { get => _studyMode; private set => _studyMode = value; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Tên node.</param>
        /// <param name="studyMode">Bắt buộc, chọn n trong k hoặc không có.</param>
        /// <param name="isRoot">Là node gốc.</param>
        /// <param name="nodeId">Id node.</param>
        /// <param name="childOfNode">Id của node cha.</param>
        /// <param name="rawHtml">Html gốc.</param>
        public ProgramFolder(string name, StudyMode studyMode, bool isRoot, string nodeId, string childOfNode, string rawHtml)
        {
            _id = nodeId;
            _childOfNode = childOfNode;
            _name = name;
            _studyMode = studyMode;
            _isRoot = isRoot;
            _rawHtml = rawHtml;
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
                _programSubjects.Add(node);
            else
                _childProgramFolders.Add(node);
        }
    }
}
