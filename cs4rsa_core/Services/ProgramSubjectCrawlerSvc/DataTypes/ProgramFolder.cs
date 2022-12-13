using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Interfaces;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System.Collections.Generic;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes
{
    public class ProgramFolder : IProgramNode
    {
        private readonly string _id;
        private readonly string _childOfNode;
        private string _name;
        private readonly List<ProgramFolder> _childProgramFolders;
        private readonly List<ProgramSubject> _childProgramSubjects;
        private readonly string _rawHtml;
        private readonly StudyMode _studyMode;
        private readonly string _description;

        public string Id => _id;
        public string ChildOfNode => _childOfNode;
        public string Name { get => _name; private set => _name = value; }
        public List<ProgramFolder> ChildProgramFolders => _childProgramFolders;
        public List<ProgramSubject> ChildProgramSubjects => _childProgramSubjects;
        public StudyMode StudyMode => _studyMode;
        public string Description => _description;
        public string RawHtml => _rawHtml;

        /// <summary>
        /// Đại diện cho một folder là một mục trong chương trình học mà bạn phải hoàn thành.
        /// Một folder sẽ chứa một tập các môn học, mà bạn cần phải học tất cả (bắt buộc) hoặc
        /// chọn n trong k môn học phải hoàn thành.
        /// </summary>
        /// <param name="name">Tên node.</param>
        /// <param name="studyMode">Bắt buộc, chọn n trong k hoặc không có.</param>
        /// <param name="isRoot">Là node gốc.</param>
        /// <param name="nodeId">Id node.</param>
        /// <param name="childOfNode">Id của node cha.</param>
        /// <param name="description">Mô tả thư mục.</param>
        /// <param name="rawHtml">Html gốc.</param>
        public ProgramFolder(string name, StudyMode studyMode, string nodeId, string childOfNode, string description, string rawHtml)
        {
            _id = nodeId;
            _childOfNode = childOfNode;
            _name = name;
            _studyMode = studyMode;
            _description = description;
            _rawHtml = rawHtml;

            _childProgramFolders = new();
            _childProgramSubjects = new();
        }

        /// <summary>
        /// Kiểm tra xem folder này đã completed hay chưa.
        /// </summary>
        /// <returns></returns>
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
                    flag = flag && subject.IsDone();
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
                if (item.IsDone() || item.StudyState == StudyState.NoHavePoint)
                    mustLearn--;
            }
            if (mustLearn == 0) return true;
            return false;
        }

        /// <summary>
        /// Trả về tất cả các ProSubject có trong folder này
        /// bằng cách gọi đệ quy đi sâu vào trong từ folder.
        /// </summary>
        /// <returns></returns>
        public List<ProgramSubject> GetProgramSubjects()
        {
            List<ProgramSubject> programSubjects = new();
            foreach (ProgramFolder folder in _childProgramFolders)
            {
                if (folder._childProgramSubjects.Count > 0)
                {
                    programSubjects.AddRange(folder._childProgramSubjects);
                }
                else
                {
                    programSubjects.AddRange(folder.GetProgramSubjects());
                }
            }
            programSubjects.AddRange(_childProgramSubjects);
            programSubjects.Sort();
            return programSubjects;
        }

        /// <summary>
        /// Kiểm tra xem danh sách các IProgramNode truyền vào có phải
        /// toàn là DbProgramSubject hay không.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private static bool AllChildIsProgramSubject(List<IProgramNode> nodes)
        {
            foreach (IProgramNode item in nodes)
            {
                if (item is not ProgramSubject)
                    return false;
            }
            return true;
        }

        public List<IProgramNode> GetAllChildNodes()
        {
            List<IProgramNode> nodes = new();
            nodes.AddRange(_childProgramFolders);
            nodes.AddRange(_childProgramSubjects);
            return nodes;
        }

        /// <summary>
        /// Lấy ra một Program Subject dựa theo subject code.
        /// </summary>
        /// <param name="subjectCode"></param>
        /// <returns></returns>
        public ProgramSubject GetProgramSubject(string subjectCode)
        {
            foreach (ProgramSubject subject in _childProgramSubjects)
            {
                if (subject.SubjectCode.Equals(subjectCode))
                {
                    return subject;
                }
            }
            foreach (ProgramFolder folder in _childProgramFolders)
            {
                ProgramSubject result = folder.GetProgramSubject(subjectCode);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Tìm kiếm một folder dựa theo name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ProgramFolder FindProgramFolder(string name)
        {
            if (_name.Equals(name))
                return this;
            else
            {
                foreach (ProgramFolder folder in _childProgramFolders)
                {
                    if (folder.FindProgramFolder(name) != null)
                    {
                        return folder;
                    }
                }
                return null;
            }
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
                HtmlDocument doc = new();
                doc.LoadHtml(_rawHtml);
                HtmlNode docNode = doc.DocumentNode;
                HtmlNode span = docNode.SelectSingleNode("//span/span");
                string spanContent = span.InnerHtml;
                string[] spanContentSlices = StringHelper.SplitAndRemoveAllSpace(spanContent);
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

        public void AddNodes(IEnumerable<ProgramSubject> nodes)
        {
            _childProgramSubjects.AddRange(nodes);
        }

        public NodeType GetNodeType()
        {
            return NodeType.Folder;
        }
    }
}
