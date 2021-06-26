using cs4rsa.Interfaces;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho hình thức học của một Folder, có thể là bắt buộc
    /// có thể là Chọn n trong k môn có trong folder.
    /// Compulisory: Bắt buộc
    /// AllowSelection: Cho phép chọn n trong k môn
    /// </summary>
    public enum StudyMode
    {
        Compulsory,
        AllowSelection
    }

    public class ProgramFolder : IProgramNode
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
        /// Kiểm tra một mã môn có tồn tại trong folder này hay không.
        /// Nó sẽ đệ quy qua tất cả các folder con bên trong.
        /// </summary>
        /// <param name="subjectCode"></param>
        /// <returns></returns>
        public bool IsExistsSubject(string subjectCode)
        {
            return false;
        }

        public bool IsCompleted()
        {
            bool flag = true;
            List<IProgramNode> allChilds = GetAllChildNodes();
            int mustComplete;
            if (AllChildIsProgramSubject(allChilds))
            {
                mustComplete = MustComplete();
                return ThisProgramSubjectIsCompleted(allChilds, mustComplete);
            }

            foreach (IProgramNode item in allChilds)
            {
                if (item is ProgramSubject)
                {
                    ProgramSubject subject = item as ProgramSubject;
                    flag = flag && subject.IsCompleted();
                }
                else
                {
                    ProgramFolder folder = item as ProgramFolder;
                    flag = flag && folder.IsCompleted();
                }
            }
            return flag;
        }

        private static bool ThisProgramSubjectIsCompleted(List<IProgramNode> subjects, int mustLearn)
        {
            foreach (ProgramSubject item in subjects)
            {
                if (item.IsCompleted())
                    mustLearn--;
            }
            if (mustLearn == 0) return true;
            return false;
        }

        /// <summary>
        /// Kiểm tra xem danh sách các IProgramNode truyền vào có phải
        /// toàn là ProgramSubject hay không.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private bool AllChildIsProgramSubject(List<IProgramNode> nodes)
        {
            foreach (IProgramNode item in nodes)
            {
                if (!(item is ProgramSubject))
                    return false;
            }
            return true;
        }

        public List<IProgramNode> GetAllChildNodes()
        {
            List<IProgramNode> nodes = new List<IProgramNode>();
            nodes.AddRange(_childProgramFolders);
            nodes.AddRange(_childProgramSubjects);
            return nodes;
        }

        /// <summary>
        /// Trả về số lượng môn học hoặc thư mục bên trong buộc phải hoàn tất
        /// để xác định rằng folder cha này đã hoàn tất hay chưa.
        /// </summary>
        /// <returns></returns>
        public int MustComplete()
        {
            if (StudyMode == StudyMode.AllowSelection)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(_rawHtml);
                HtmlNode docNode = doc.DocumentNode;
                HtmlNode span = docNode.SelectSingleNode("//span/span");
                string spanContent = span.InnerHtml;
                string[] spanContentSlices = Helpers.StringHelper.SplitAndRemoveAllSpace(spanContent);
                int mustComplete = int.Parse(spanContentSlices[1]);
                return mustComplete;
            }
            else
            {
                return _childProgramFolders.Count + _childProgramSubjects.Count;
            }
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
