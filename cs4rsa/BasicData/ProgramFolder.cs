using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public class ProgramFolder
    {
        private string _name;
        private List<ProgramFolder> _childProgramFolders;
        private List<ProgramSubject> _programSubject;
        private bool _isRoot;
        private string _rawHtml;
        private StudyMode _studyMode;

        public string Name { get => _name; private set => _name = value; }
        public List<ProgramFolder> ChildProgramFolder { get => _childProgramFolders; private set => _childProgramFolders = value; }
        public List<ProgramSubject> ProgramSubjects { get => _programSubject; private set => _programSubject = value; }
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
        public ProgramFolder(string name, StudyMode studyMode, bool isRoot, string nodeId, string childOfNode,string rawHtml)
        {
            _name = name;
            _studyMode = studyMode;
            _isRoot = isRoot;
            _rawHtml = rawHtml;
        }

        /// <summary>
        /// Kiểm tra folder này đã hoàn tất hay chưa,
        /// folder này hoàn tất hay không phụ thuộc hoàn thoàn vào
        /// việc các môn và các folder bên trong đã hoàn tất hay chưa.
        /// </summary>
        /// <returns></returns>
        private bool IsComplete()
        {
            return false;
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
    }
}
