using cs4rsa.BasicData;
using cs4rsa.Enums;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;


namespace cs4rsa.Crawler
{
    /// <summary>
    /// Bộ cào chương trình học của sinh viên.
    /// ** Không chạy sau 23h đó là lúc server DTU bảo trì.
    /// ** Đảm bảo nó chạy trong một lần duy nhất.
    /// Bộ cào này lưu thông tin của các môn có trong chương trình học của bạn
    /// vào database để vào lần chạy tiếp theo người dùng gần như không phải nhập lại sessionid
    /// trừ khi họ Đăng xuất và đăng nhập với một tài khoảng mới.
    /// </summary>
    public class StudentProgramCrawler
    {
        private string _sessionId;
        public List<HtmlNode> _fileNodes;
        public List<HtmlNode> _folderNodes;
        public List<ProgramFolder> ProgramFolders;
        public List<ProgramSubject> ProgramSubjects;
        public ProgramFolder Root;
        /// <summary>
        /// Nhận vào một chuỗi đặc biệt được mã hoá từ mã sinh viên.
        /// </summary>
        /// <param name="specialString">Mã đặc biệt được mã hoá từ mã sinh viên.</param>
        public StudentProgramCrawler(string sessionId, string url)
        {
            _sessionId = sessionId;
            List<HtmlNode> allNodes = GetAllTrTag(url);
            GetFileAnhFolderNodes(allNodes);
            ProgramFolders = GetProgramFolders(_folderNodes);
            ProgramSubjects = GetProgramSubjects(_fileNodes);
            List<ProgramFolder> divedProgramFolders = DivChildNode(ProgramFolders, ProgramSubjects);
            Root = MergeNode(divedProgramFolders);
        }

        /// <summary>
        /// Gộp tất cả các folder thành một cây thư mục
        /// </summary>
        /// <param name="divedProgramFolders"></param>
        /// <returns></returns>
        private ProgramFolder MergeNode(List<ProgramFolder> divedProgramFolders)
        {
            List<ProgramFolder> SortedList = divedProgramFolders.OrderBy(o => o.ChildOfNode).ToList();
            SortedList.Reverse();
            string childOfNodeId = SortedList[0].ChildOfNode;
            while (SortedList.Count > 1)
            {
                ProgramFolder parrentFolder = FindParrentNode(SortedList, childOfNodeId);
                List<ProgramFolder> childFolderNodes = GetChildFolderNodes(parrentFolder, SortedList);
                foreach (ProgramFolder node in childFolderNodes)
                {
                    parrentFolder.AddNode(node);
                    SortedList.Remove(node);
                }
                SortedList = SortedList.OrderBy(o => o.ChildOfNode).ToList();
                SortedList.Reverse();
                childOfNodeId = SortedList[0].ChildOfNode;
            }
            return SortedList[0];
        }

        /// <summary>
        /// Tìm một node có id trong danh sách các node.
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="childOfNodeId"></param>
        /// <returns></returns>
        private ProgramFolder FindParrentNode(List<ProgramFolder> nodes, string childOfNodeId)
        {
            foreach (ProgramFolder folder in nodes)
            {
                if (folder.GetIdNode() == childOfNodeId)
                {
                    return folder;
                }
            }
            return null;
        }

        /// <summary>
        /// Copy các ProgramSubject con vào các ProgramFolder.
        /// </summary>
        /// <param name="nodes">Danh sách tất cả các node bao gồm cả subject và folder.</param>
        private List<ProgramFolder> DivChildNode(List<ProgramFolder> folderNodes, List<ProgramSubject> subjectNodes)
        {
            List<ProgramFolder> programFolders = new List<ProgramFolder>();
            foreach (ProgramFolder node in folderNodes)
            {
                List<ProgramSubject> subjects = GetChildProgramSubject(node, subjectNodes);
                node.AddNodes(subjects);
                programFolders.Add(node);
            }
            return programFolders;
        }


        /// <summary>
        /// Lấy ra tất cả các subject con thuộc folder.
        /// </summary>
        /// <param name="parrent">Một folder node.</param>
        /// <param name="subjects">Danh sách các subject node.</param>
        /// <returns></returns>
        private List<ProgramSubject> GetChildProgramSubject(ProgramFolder parrent, List<ProgramSubject> subjects)
        {
            List<ProgramSubject> childSubjects = new List<ProgramSubject>();
            foreach (ProgramSubject subject in subjects)
            {
                if (subject.GetChildOfNode() == parrent.GetIdNode())
                    childSubjects.Add(subject);
            }
            return childSubjects;
        }

        public List<HtmlNode> GetAllTrTag(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            List<HtmlNode> trTags = doc.DocumentNode.SelectNodes("//tr").ToList();
            trTags.RemoveAt(0);
            return trTags;
        }

        /// <summary>
        /// Chia list node thành các fileNode và FolderNode.
        /// </summary>
        /// <param name="trNodes">Danh sách tất cả các tr node trong chương trình học.</param>
        public void GetFileAnhFolderNodes(List<HtmlNode> trNodes)
        {
            List<HtmlNode> folderNodes = new List<HtmlNode>();
            List<HtmlNode> fileNodes = new List<HtmlNode>();
            foreach (HtmlNode node in trNodes)
            {
                string innerHtml = node.InnerHtml;
                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(innerHtml);
                HtmlNode rootNode = htmlDocument.DocumentNode;
                HtmlNode spanFolderNode = rootNode.SelectSingleNode("//span[@class='folder']");
                HtmlNode spanFileNode = rootNode.SelectSingleNode("//span[@class='file']");
                if (spanFolderNode != null && spanFolderNode.Attributes["class"].Value == "folder")
                    folderNodes.Add(node);
                if (spanFileNode != null)
                    fileNodes.Add(node);
            }
            _folderNodes = folderNodes;
            _fileNodes = fileNodes;
        }

        public List<ProgramFolder> GetProgramFolders(List<HtmlNode> folderNodes)
        {
            List<ProgramFolder> programFolders = new List<ProgramFolder>();
            foreach (HtmlNode htmlNode in folderNodes)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlNode.InnerHtml);
                HtmlNode docNode = doc.DocumentNode;

                string idValue = htmlNode.Attributes["id"].Value;
                string classValue = htmlNode.Attributes["class"].Value;
                string childOfNode;
                string id = idValue.Split(new char[] { '-' })[1];
                if (classValue.Equals("toptitle"))
                    childOfNode = "0";
                else
                    childOfNode = classValue.Split(new char[] { '-' })[3];

                HtmlNode studyModeSpanNode = docNode.SelectSingleNode("//span/span");
                StudyMode studyMode;
                if (studyModeSpanNode != null)
                {
                    string studyModeText = studyModeSpanNode.InnerText;
                    if (studyModeText.Equals("(Bắt buộc)"))
                        studyMode = StudyMode.Compulsory;
                    else
                        studyMode = StudyMode.AllowSelection;
                }
                else
                    studyMode = StudyMode.Compulsory;

                string name = GetNameFolderNode(htmlNode);

                ProgramFolder folder = new ProgramFolder(name, studyMode, id, childOfNode, htmlNode.InnerHtml);
                programFolders.Add(folder);
            }
            return programFolders;
        }

        public List<ProgramSubject> GetProgramSubjects(List<HtmlNode> fileNodes)
        {
            List<ProgramSubject> programSubjects = new List<ProgramSubject>();
            foreach (HtmlNode node in fileNodes)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                HtmlNode docNode = doc.DocumentNode;

                // id and child of node
                string idValue = node.Attributes["id"].Value;
                string[] classSlices = Helpers.StringHelper.SplitAndRemoveAllSpace(node.Attributes["class"].Value);
                string classValue = classSlices[0];
                string childOfNode;
                string id = idValue.Split(new char[] { '-' })[1];
                if (classValue.Equals("toptitle"))
                    childOfNode = "0";
                else
                    childOfNode = classValue.Split(new char[] { '-' })[3];

                // other infomations
                HtmlNode aTag = docNode.SelectSingleNode("//span/a");
                string subjectCode = Helpers.StringHelper.SuperCleanString(aTag.InnerText);
                string name = aTag.Attributes["title"].Value;

                // course id
                string urlToDetailPage = aTag.Attributes["href"].Value;
                string[] splits = urlToDetailPage.Split(new char[] { '&' });
                string courseId = splits[1].Split(new char[] { '=' })[1];

                //study unit
                HtmlNode tdStudyUnitTag = docNode.SelectSingleNode("//td[3]");
                string studyUnit = Helpers.StringHelper.SuperCleanString(tdStudyUnitTag.InnerText);

                //studyunit type
                HtmlNode tdStudyUnitTypeTag = docNode.SelectSingleNode("//td[4]");
                string studyUnitTypeString = Helpers.StringHelper.SuperCleanString(tdStudyUnitTypeTag.InnerText);
                StudyUnitType studyUnitType = Helpers.BasicDataConverter.ToStudyUnitType(studyUnitTypeString);

                //prerequisiteSubjects and parallelSubjects
                DTUSubjectCrawler crawler = new DTUSubjectCrawler(_sessionId, courseId);
                List<string> prerequisiteSubjects = crawler.PrerequisiteSubjects;
                List<string> parallelSubjects = crawler.ParallelSubjects;

                // study state
                StudyState studyState;
                string nodeContent = Helpers.StringHelper.SuperCleanString(node.InnerText);
                if (nodeContent.Contains("Đã hoàn tất"))
                    studyState = StudyState.Completed;
                else
                {
                    if (nodeContent.Contains("Chưa có Điểm"))
                        studyState = StudyState.NoHavePoint;
                    else
                        studyState = StudyState.UnLearned;
                }


                ProgramSubject subject = new ProgramSubject(id, childOfNode, subjectCode, name, studyUnit,
                    studyUnitType, prerequisiteSubjects, parallelSubjects, studyState);
                programSubjects.Add(subject);
            }
            return programSubjects;
        }

        /// <summary>
        /// Trả về tên của một folderNode.
        /// </summary>
        /// <param name="folderNode">Một html folder node.</param>
        /// <returns></returns>
        private string GetNameFolderNode(HtmlNode folderNode)
        {
            string html = folderNode.InnerHtml;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode nodesToRemove = docNode.SelectSingleNode("//span/span");
            if (nodesToRemove != null)
                nodesToRemove.Remove();
            HtmlNode span = docNode.SelectSingleNode("//span");
            return Helpers.StringHelper.SuperCleanString(span.InnerText);
        }

        /// <summary>
        /// Lấy ra chuỗi nhằm xác định studyMode là gì,
        /// chúng có thể là Bắt buộc, Chọn n trong k môn hoặc không có.
        /// </summary>
        /// <param name="folderNode">Một html folder node.</param>
        /// <returns></returns>
        private string GetStudyModeText(HtmlNode folderNode)
        {
            string html = folderNode.InnerHtml;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode span = docNode.SelectSingleNode("//span/span");
            return Helpers.StringHelper.SuperCleanString(span.InnerText);
        }

        private List<ProgramFolder> GetChildFolderNodes(ProgramFolder parrent, List<ProgramFolder> programNodes)
        {
            List<ProgramFolder> childProgramNodes = new List<ProgramFolder>();
            foreach (ProgramFolder node in programNodes)
            {
                if (node.GetChildOfNode() == parrent.GetIdNode())
                    childProgramNodes.Add(node);
            }
            return childProgramNodes;
        }
    }
}
