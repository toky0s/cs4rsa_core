using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Interfaces;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes
{
    public class ProgramFolder : IProgramNode
    {
        private readonly string _id;
        private readonly string _childOfNode;
        private readonly string _name;
        private readonly string _description;
        private readonly string _rawHtml;
        private bool _completed;
        private readonly List<ProgramFolder> _childProgramFolders;
        private readonly List<ProgramSubject> _childProgramSubjects;
        private readonly StudyMode _studyMode;

        public string Id => _id;
        public string Name => _name;
        public string Description => _description;
        public bool Completed
        {
            get
            {
                return _completed;
            }
            set
            {
                _completed = value;
            }
        }

        public List<ProgramFolder> ChildProgramFolders => _childProgramFolders;
        public List<ProgramSubject> ChildProgramSubjects => _childProgramSubjects;
        public StudyMode StudyMode => _studyMode;

        /// <summary>
        /// Đại diện cho một folder là một mục trong chương trình học mà bạn phải hoàn thành.
        /// Một folder sẽ chứa một tập các môn học, mà bạn cần phải học tất cả (bắt buộc) hoặc
        /// chọn n trong k môn học phải hoàn thành.
        /// </summary>
        /// <param name="name">Tên node.</param>
        /// <param name="studyMode">Bắt buộc, chọn n trong k hoặc không có.</param>
        /// <param name="isRoot">Là node gốc.</param>
        /// <param name="id">Id node.</param>
        /// <param name="childOfNode">Id của node cha.</param>
        /// <param name="description">Mô tả thư mục.</param>
        /// <param name="rawHtml">Html gốc.</param>
        public ProgramFolder(string name, StudyMode studyMode, string id, string childOfNode, string description, string rawHtml)
        {
            _id = id;
            _childOfNode = childOfNode;
            _name = name;
            _studyMode = studyMode;
            _description = description;
            _childProgramFolders = new();
            _childProgramSubjects = new();
            _rawHtml = rawHtml;
        }

        [JsonConstructor]
        public ProgramFolder(
            string name,
            string id,
            string childOfNode,
            string description,
            bool completed,
            StudyMode studyMode,
            List<ProgramFolder> childProgramFolders,
            List<ProgramSubject> childProgramSubjects
        )
        {
            _name = name;
            _studyMode = studyMode;
            _id = id;
            _childOfNode = childOfNode;
            _description = description;
            _completed = completed;
            _childProgramFolders = childProgramFolders;
            _childProgramSubjects = childProgramSubjects;
        }

        /// <summary>
        /// Kiểm tra xem folder này đã completed hay chưa.
        /// </summary>
        /// <returns></returns>
        public bool IsCompleted()
        {
            bool flag = true;
            List<IProgramNode> allChilds = GetAllChildNodes();
            if (AllChildIsProgramSubject(allChilds))
            {
                return ThisProgramSubjectIsCompleted(allChilds);
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

        /// <summary>
        /// Xét một Folder chỉ chứa Subject.
        /// Folder này hoàn thành khi số lượng Subject hoàn thành
        /// bằng với số lượng mustLearn được truyền vào.
        /// </summary>
        /// <param name="subjects">Danh sách </param>
        /// <param name="mustLearn">Số lượng một học cần phải hoàn tất</param>
        /// <returns></returns>
        private bool ThisProgramSubjectIsCompleted(List<IProgramNode> subjects)
        {
            int mustLearn = MustComplete(_rawHtml);
            foreach (ProgramSubject item in subjects.Cast<ProgramSubject>())
            {
                if (item.IsDone() || item.StudyState == StudyState.NoHavePoint)
                    mustLearn--;
            }
            return mustLearn == 0;
        }

        /// <summary>
        /// Kiểm tra xem danh sách các IProgramNode truyền vào có phải
        /// toàn là DbProgramSubjects hay không.
        /// </summary>
        /// <param name="nodes">Danh sách các node</param>
        /// <returns>Danh sách node</returns>
        private static bool AllChildIsProgramSubject(IEnumerable<IProgramNode> nodes)
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
        /// <returns>Số lượng môn học hoặc thư mục bên trong buộc phải hoàn tất</returns>
        public int MustComplete(string rawHtml)
        {
            if (StudyMode == StudyMode.AllowSelection)
            {
                HtmlDocument doc = new();
                doc.LoadHtml(rawHtml);
                HtmlNode docNode = doc.DocumentNode;
                HtmlNode span = docNode.SelectSingleNode("//span/span");
                string spanContent = span.InnerHtml;
                string[] spanContentSlices = StringHelper.SplitAndRemoveAllSpace(spanContent);
                return int.Parse(spanContentSlices[1]);
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

        public NodeType GetNodeType()
        {
            return NodeType.Folder;
        }
    }
}
