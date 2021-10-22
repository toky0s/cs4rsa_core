using Cs4rsaDatabaseService.DataProviders;
using Cs4rsaDatabaseService.Models;
using HelperService;
using HtmlAgilityPack;
using ProgramSubjectCrawlerService.DataTypes;
using ProgramSubjectCrawlerService.DataTypes.Enums;
using SubjectCrawlService1.Crawlers.Interfaces;
using SubjectCrawlService1.DataTypes;
using SubjectCrawlService1.DataTypes.Enums;
using SubjectCrawlService1.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgramSubjectCrawlerService.Crawlers
{
    public class StudentProgramCrawler
    {
        private string _sessionId;
        private Cs4rsaDbContext _cs4rsaDbContext;
        private IPreParSubjectCrawler _preParSubjectCrawler;

        public List<HtmlNode> _fileNodes;
        public List<HtmlNode> _folderNodes;
        public List<ProgramFolder> ProgramFolders;
        public List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject> ProgramSubjects;
        public ProgramFolder Root;
        public StudentProgramCrawler(string sessionId, Cs4rsaDbContext cs4rsaDbContext, IPreParSubjectCrawler preParSubjectCrawler)
        {
            _sessionId = sessionId;
            _cs4rsaDbContext = cs4rsaDbContext;
            _preParSubjectCrawler = preParSubjectCrawler;
        }

        public async Task GetNode(string url)
        {
            List<HtmlNode> allNodes = await GetAllTrTag(url);
            GetFileAnhFolderNodes(allNodes);
            ProgramFolders = GetProgramFolders(_folderNodes);
            ProgramSubjects = await GetProgramSubjects(_fileNodes);
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
        private static ProgramFolder FindParrentNode(List<ProgramFolder> nodes, string childOfNodeId)
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
        private List<ProgramFolder> DivChildNode(List<ProgramFolder> folderNodes, List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject> subjectNodes)
        {
            List<ProgramFolder> programFolders = new List<ProgramFolder>();
            foreach (ProgramFolder node in folderNodes)
            {
                List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject> subjects = GetChildProgramSubject(node, subjectNodes);
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
        private List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject> GetChildProgramSubject(ProgramFolder parrent, List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject> subjects)
        {
            List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject> childSubjects = new List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject>();
            foreach (ProgramSubjectCrawlerService.DataTypes.ProgramSubject subject in subjects)
            {
                if (subject.GetChildOfNode() == parrent.GetIdNode())
                    childSubjects.Add(subject);
            }
            return childSubjects;
        }

        public async Task<List<HtmlNode>> GetAllTrTag(string url)
        {
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = await web.LoadFromWebAsync(url);
            List<HtmlNode> trTags = doc.DocumentNode.SelectNodes("//tr").ToList();
            trTags.RemoveAt(0);
            return trTags;
        }

        /// <summary>
        /// Trả về trang html chương trình học
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async static Task<HtmlDocument> FetchHtml(string url)
        {
            Task<HtmlDocument> fetchHtmlTask = new Task<HtmlDocument>(() =>
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                return doc;
            });
            fetchHtmlTask.Start();
            HtmlDocument htmlDocument = await fetchHtmlTask;
            return htmlDocument;
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
                string description;
                if (studyModeSpanNode != null)
                {
                    string studyModeText = studyModeSpanNode.InnerText;
                    if (studyModeText.Equals("(Bắt buộc)"))
                    {
                        studyMode = StudyMode.Compulsory;
                        description = "Bắt buộc";
                    }
                    else
                    {
                        studyMode = StudyMode.AllowSelection;
                        description = studyModeText;
                    }
                }
                else
                {
                    studyMode = StudyMode.Compulsory;
                    description = "Bắt buộc";
                }
                string name = GetNameFolderNode(htmlNode);

                ProgramFolder folder = new ProgramFolder(name, studyMode, id, childOfNode, description, htmlNode.InnerHtml);
                programFolders.Add(folder);
            }
            return programFolders;
        }

        public async Task<List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject>> GetProgramSubjects(List<HtmlNode> fileNodes)
        {
            List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject> programSubjects = new List<ProgramSubjectCrawlerService.DataTypes.ProgramSubject>();
            foreach (HtmlNode node in fileNodes)
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(node.InnerHtml);
                HtmlNode docNode = doc.DocumentNode;

                // id and child of node
                string idValue = node.Attributes["id"].Value;
                string[] classSlices = StringHelper.SplitAndRemoveAllSpace(node.Attributes["class"].Value);
                string classValue = classSlices[0];
                string childOfNode;
                string id = idValue.Split(new char[] { '-' })[1];
                if (classValue.Equals("toptitle"))
                    childOfNode = "0";
                else
                    childOfNode = classValue.Split(new char[] { '-' })[3];

                string parrentNodeName = GetParrentNodeWithId(childOfNode);

                // other infomations
                HtmlNode aTag = docNode.SelectSingleNode("//span/a");
                string subjectCode = StringHelper.SuperCleanString(aTag.InnerText);
                string name = aTag.Attributes["title"].Value;

                // course id
                string urlToDetailPage = aTag.Attributes["href"].Value;
                string[] splits = urlToDetailPage.Split(new char[] { '&' });
                string courseId = splits[1].Split(new char[] { '=' })[1];

                //study unit
                HtmlNode tdStudyUnitTag = docNode.SelectSingleNode("//td[3]");
                string studyUnit = StringHelper.SuperCleanString(tdStudyUnitTag.InnerText);

                //studyunit type
                HtmlNode tdStudyUnitTypeTag = docNode.SelectSingleNode("//td[4]");
                string studyUnitTypeString = StringHelper.SuperCleanString(tdStudyUnitTypeTag.InnerText);
                StudyUnitType studyUnitType = BasicDataConverter.ToStudyUnitType(studyUnitTypeString);

                //prerequisiteSubjects and parallelSubjects
                PreParContainer preParContainer;
                if (_sessionId != null)
                {
                    preParContainer = await _preParSubjectCrawler.Run(courseId, _sessionId);
                }
                else
                {
                    preParContainer = await _preParSubjectCrawler.Run(_sessionId);
                }
                List<string> prerequisiteSubjects = preParContainer.PrerequisiteSubjects;
                List<string> parallelSubjects = preParContainer.ParallelSubjects;
                List<string> sPreParSubjects = prerequisiteSubjects.Concat(parallelSubjects).ToList();
                List<Cs4rsaDatabaseService.Models.PreParSubject> preParSubjects = new();
                sPreParSubjects.ForEach(
                    sPreParSubject => {
                        Cs4rsaDatabaseService.Models.PreParSubject preParSubject = new Cs4rsaDatabaseService.Models.PreParSubject()
                        {
                            SubjectCode = sPreParSubject
                        };
                        preParSubjects.Add(preParSubject);
                    }
                );

                // study state
                StudyState studyState;
                string nodeContent = StringHelper.SuperCleanString(node.InnerText);
                if (nodeContent.Contains("Đã hoàn tất"))
                    studyState = StudyState.Completed;
                else
                {
                    if (nodeContent.Contains("Chưa có Điểm"))
                        studyState = StudyState.NoHavePoint;
                    else
                        studyState = StudyState.UnLearned;
                }

                ProgramSubjectCrawlerService.DataTypes.ProgramSubject subject = new ProgramSubjectCrawlerService.DataTypes.ProgramSubject(id, childOfNode, subjectCode, name, studyUnit,
                    studyUnitType, prerequisiteSubjects, parallelSubjects, studyState, courseId, parrentNodeName);
                programSubjects.Add(subject);
                Cs4rsaDatabaseService.Models.ProgramSubject programSubject = new()
                {
                    SubjectCode = subjectCode,
                    CourseId = courseId,
                    Name = name,
                    Credit = studyUnit,
                };

                _cs4rsaDbContext.ProgramSubjects.Add(programSubject);
                foreach (Cs4rsaDatabaseService.Models.PreParSubject preParSubject in preParSubjects)
                {
                    _cs4rsaDbContext.PreParSubjects.Add(preParSubject);
                    PreProDetail preProDetail = new()
                    {
                        ProgramSubject = programSubject,
                        PreParSubject = preParSubject
                    };
                    _cs4rsaDbContext.PreProDetails.Add(preProDetail);

                    ParProDetail parProDetail = new()
                    {
                        ProgramSubject = programSubject,
                        PreParSubject = preParSubject
                    };
                    _cs4rsaDbContext.ParProDetails.Add(parProDetail);
                }
                _cs4rsaDbContext.SaveChanges();
                //_subjectSaver.Save(subject);
            }
            return programSubjects;
        }

        private string GetParrentNodeWithId(string id)
        {
            foreach (ProgramFolder programFolder in ProgramFolders)
            {
                if (programFolder.Id.Equals(id))
                    return programFolder.Name;
            }
            return "";
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
            return StringHelper.SuperCleanString(span.InnerText);
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
            return StringHelper.SuperCleanString(span.InnerText);
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
