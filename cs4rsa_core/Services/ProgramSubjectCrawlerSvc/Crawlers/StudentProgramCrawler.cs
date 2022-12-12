using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Crawlers.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Utils;
using Cs4rsa.Utils;
using Cs4rsa.Utils.Interfaces;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using PreParSubject = Cs4rsa.Cs4rsaDatabase.Models.PreParSubject;
using ProgramSubject = Cs4rsa.Cs4rsaDatabase.Models.ProgramSubject;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers
{
    public class StudentProgramCrawler
    {
        private static string _sessionId;
        private static string _studentId;

        // Transient
        private List<HtmlNode> _fileNodes;
        private List<HtmlNode> _folderNodes;
        private HtmlNodeCollection _trNodes;

        private IEnumerable<ProgramFolder> ProgramFolders;
        private IEnumerable<DataTypes.ProgramSubject> ProgramSubjects;
        private ProgramFolder Root;

        #region DI
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPreParSubjectCrawler _preParSubjectCrawler;
        #endregion

        public StudentProgramCrawler(
            IUnitOfWork unitOfWork,
            IPreParSubjectCrawler preParSubjectCrawler
        )
        {
            _unitOfWork = unitOfWork;
            _preParSubjectCrawler = preParSubjectCrawler;
        }

        /// <summary>
        /// Set thông tin trước khi thực hiện lấy chương trình học.
        /// </summary>
        /// <param name="sessionId">Session ID</param>
        /// <param name="studentId">Student ID</param>
        public static void SetInfor(string sessionId, string studentId)
        {
            _sessionId = sessionId;
            _studentId = studentId;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="specialString"></param>
        /// <param name="t"></param>
        /// <param name="nodeName"></param>
        /// <param name="curid"></param>
        /// <returns></returns>
        public async Task<ProgramFolder> GetNode(
            string specialString,
            string t,
            string nodeName,
            int curid
        )
        {
            string url = LoadChuongTrinhHocEachPart(specialString, t, nodeName, curid);
            HtmlNodeCollection allNodes = await GetAllTrTag(url, nodeName, true);
            GetFileAnhFolderNodes(allNodes);
            ProgramFolders = GetProgramFolders(_folderNodes);
            ProgramSubjects = await GetProgramSubjects(_fileNodes);
            IEnumerable<ProgramFolder> divedProgramFolders = DivChildNode(ProgramFolders, ProgramSubjects);
            Root = MergeNode(divedProgramFolders);
            return Root;
        }

        public async Task<ProgramFolder> GetNodeA(
            string specialString,
            string t,
            string nodeName,
            int curid
        )
        {
            string url = LoadChuongTrinhHocEachPart(specialString, t, nodeName, curid);
            _trNodes = await GetAllTrTag(url, nodeName, true);
            ProgramFolder programFolder = GetProgramFolder(_trNodes[0]);
            Root = await GetFolderNodeA(1, programFolder);
            return Root;
        }

        /// <summary>
        /// Lấy ra đường dẫn tới phần học
        /// </summary>
        /// <param name="specialString">Mã đặc biệt</param>
        /// <param name="t">Epoch</param>
        /// <param name="curid">Mã ngành</param>
        /// <param name="cursectionid">PhanHoc</param>
        /// <returns></returns>
        private static string LoadChuongTrinhHocEachPart(
            string specialString,
            string t,
            string nodeName,
            int curid
        )
        {
            return $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={specialString}&acaLevid=3&curid={curid}&cursectionid={nodeName}";
        }

        /// <summary>
        /// Gộp tất cả các folder thành một cây thư mục
        /// </summary>
        /// <param name="divedProgramFolders">List of ProgramFolder</param>
        /// <returns>ProgramFolder</returns>
        private static ProgramFolder MergeNode(IEnumerable<ProgramFolder> divedProgramFolders)
        {
            List<ProgramFolder> sortedList = divedProgramFolders.OrderBy(o => o.ChildOfNode).ToList();
            sortedList.Reverse();
            string childOfNodeId = sortedList[0].ChildOfNode;
            while (sortedList.Count > 1)
            {
                ProgramFolder parrentFolder = FindParrentNode(sortedList, childOfNodeId);
                IEnumerable<ProgramFolder> childFolderNodes = GetChildFolderNodes(parrentFolder, sortedList);
                foreach (ProgramFolder node in childFolderNodes)
                {
                    parrentFolder.AddNode(node);
                    sortedList.Remove(node);
                }
                sortedList = sortedList.OrderBy(o => o.ChildOfNode).ToList();
                sortedList.Reverse();
                childOfNodeId = sortedList[0].ChildOfNode;
            }
            return sortedList.First();
        }

        /// <summary>
        /// Tìm một node có ID trong danh sách các node.
        /// </summary>
        /// <param name="nodes">Danh sách các Node</param>
        /// <param name="childOfNodeId">ID node con</param>
        /// <returns>ProgramFolder</returns>
        private static ProgramFolder FindParrentNode(IEnumerable<ProgramFolder> nodes, string childOfNodeId)
        {
            return nodes.Where(n => n.GetIdNode() == childOfNodeId).FirstOrDefault();
        }

        /// <summary>
        /// Copy các ProgramSubject con vào các ProgramFolder.
        /// </summary>
        /// <param name="nodes">Danh sách tất cả các node bao gồm cả subject và folder.</param>
        private static IEnumerable<ProgramFolder> DivChildNode(IEnumerable<ProgramFolder> folderNodes, IEnumerable<DataTypes.ProgramSubject> subjectNodes)
        {
            foreach (ProgramFolder node in folderNodes)
            {
                IEnumerable<DataTypes.ProgramSubject> subjects = GetChildProgramSubject(node, subjectNodes);
                node.AddNodes(subjects);
                yield return node;
            }
        }

        /// <summary>
        /// Lấy ra tất cả các subject con thuộc folder.
        /// </summary>
        /// <param name="parrent">Một folder node.</param>
        /// <param name="subjects">Danh sách các subject node.</param>
        /// <returns></returns>
        private static IEnumerable<DataTypes.ProgramSubject> GetChildProgramSubject(ProgramFolder parrent, IEnumerable<DataTypes.ProgramSubject> subjects)
        {
            foreach (DataTypes.ProgramSubject subject in subjects)
            {
                if (subject.GetChildOfNode() == parrent.GetIdNode())
                {
                    yield return subject;
                }
            }
        }

        private static async Task<HtmlNodeCollection> GetAllTrTag(
            string url,
            string nodeName,
            bool isUseCache
        )
        {
            HtmlWeb web = new();
            HtmlDocument doc = new();
            string path = Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PROGRAMS, _studentId, nodeName + ".html");
            if (File.Exists(path) && isUseCache)
            {
                doc.Load(path);
            }
            else
            {
                doc = await web.LoadFromWebAsync(url);
                await File.WriteAllTextAsync(path, doc.DocumentNode.InnerHtml);
            }
            HtmlNodeCollection trTags = doc.DocumentNode.SelectNodes("//tr");
            trTags.RemoveAt(0);
            return trTags;
        }

        /// <summary>
        /// Deprecated
        /// Chia list node thành các fileNode và FolderNode.
        /// </summary>
        /// <param name="trNodes">Danh sách tất cả các tr node trong chương trình học.</param>
        private void GetFileAnhFolderNodes(HtmlNodeCollection trNodes)
        {
            List<HtmlNode> folderNodes = new();
            List<HtmlNode> fileNodes = new();
            foreach (HtmlNode node in trNodes)
            {
                string innerHtml = node.InnerHtml;
                HtmlDocument htmlDocument = new();
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

        /// <summary>
        /// Deprecated
        /// Chia list node thành các fileNode và FolderNode.
        /// </summary>
        private async Task<ProgramFolder> GetFolderNodeA(int index, ProgramFolder parentFolderNode)
        {
            if (index == _trNodes.Count - 1)
            {
                return parentFolderNode;
            }

            HtmlNode node = _trNodes[index];
            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(node.InnerHtml);
            HtmlNode rootNode = htmlDocument.DocumentNode;

            HtmlNode spanFolderNode = rootNode.SelectSingleNode("//span[@class='folder']");
            if (spanFolderNode != null && spanFolderNode.Attributes["class"].Value == "folder")
            {
                HtmlDocument doc = new();
                doc.LoadHtml(node.InnerHtml);
                HtmlNode docNode = doc.DocumentNode;

                string idValue = node.Attributes["id"].Value;
                string classValue = node.Attributes["class"].Value;
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
                string name = GetNameFolderNode(node);
                ProgramFolder programFolder = new(name, studyMode, id, childOfNode, description, node.InnerHtml);
                programFolder = await GetFolderNodeA(index + 1, programFolder);
                parentFolderNode.ChildProgramFolders.Add(programFolder);
                return programFolder;
            }
            
            HtmlNode spanFileNode = rootNode.SelectSingleNode("//span[@class='file']");
            if (spanFileNode != null)
            {
                parentFolderNode.ChildProgramSubjects.Add(await GetProgramSubject(node));
            }
            
            return await GetFolderNodeA(index + 1, parentFolderNode); ;
        }

        /// <summary>
        /// Chia list node thành các fileNode và FolderNode.
        /// </summary>
        /// <param name="trNodes">Danh sách tất cả các tr node trong chương trình học.</param>
        private void GetFolderNodes(HtmlNodeCollection trNodes)
        {
            List<HtmlNode> folderNodes = new();
            List<HtmlNode> fileNodes = new();
            foreach (HtmlNode node in trNodes)
            {
                string innerHtml = node.InnerHtml;
                HtmlDocument htmlDocument = new();
                htmlDocument.LoadHtml(innerHtml);
                HtmlNode rootNode = htmlDocument.DocumentNode;
                HtmlNode spanFolderNode = rootNode.SelectSingleNode("//span[@class='folder']");
                HtmlNode spanFileNode = rootNode.SelectSingleNode("//span[@class='file']");
                if (spanFolderNode != null && spanFolderNode.Attributes["class"].Value == "folder")
                {
                    GetProgramFolder(node);
                }
                if (spanFileNode != null)
                {
                    fileNodes.Add(node);
                }
            }
            _folderNodes = folderNodes;
            _fileNodes = fileNodes;
        }

        /// <summary>
        /// Deprecate
        /// </summary>
        /// <param name="folderNodes"></param>
        /// <returns></returns>
        private static IEnumerable<ProgramFolder> GetProgramFolders(IEnumerable<HtmlNode> folderNodes)
        {
            foreach (HtmlNode htmlNode in folderNodes)
            {
                HtmlDocument doc = new();
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
                yield return new(name, studyMode, id, childOfNode, description, htmlNode.InnerHtml);
            }
        }

        private static ProgramFolder GetProgramFolder(HtmlNode trNode)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(trNode.InnerHtml);
            HtmlNode docNode = doc.DocumentNode;

            string idValue = trNode.Attributes["id"].Value;
            string classValue = trNode.Attributes["class"].Value;
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
            string name = GetNameFolderNode(trNode);
            return new(name, studyMode, id, childOfNode, description, trNode.InnerHtml);
        }

        private async Task<DataTypes.ProgramSubject> GetProgramSubject(HtmlNode node)
        {
            HtmlDocument doc = new();
            doc.LoadHtml(node.InnerHtml);
            HtmlNode docNode = doc.DocumentNode;

            // id and child of node
            string idValue = node.Attributes["id"].Value;
            string[] classSlices = StringHelper.SplitAndRemoveAllSpace(node.Attributes["class"].Value);
            string classValue = classSlices[0];
            string childOfNode;
            string id = idValue.Split(new char[] { '-' })[1];
            if (classValue.Equals("toptitle", StringComparison.Ordinal))
            {
                childOfNode = "0";
            }
            else
            {
                childOfNode = classValue.Split(new char[] { '-' })[3];
            }

            //string parrentNodeName = GetParrentNodeWithId(childOfNode);

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
            int studyUnit = int.Parse(StringHelper.SuperCleanString(tdStudyUnitTag.InnerText));

            //studyunit type
            HtmlNode tdStudyUnitTypeTag = docNode.SelectSingleNode("//td[4]");
            string studyUnitTypeString = StringHelper.SuperCleanString(tdStudyUnitTypeTag.InnerText);
            StudyUnitType studyUnitType = BasicDataConverter.ToStudyUnitType(studyUnitTypeString);

            //prerequisiteSubjects and parallelSubjects
            PreParContainer preParContainer;
            if (_sessionId != null && _sessionId != string.Empty)
            {
                preParContainer = await _preParSubjectCrawler.Run(courseId, _sessionId);
            }
            else
            {
                preParContainer = await _preParSubjectCrawler.Run(_sessionId);
            }
            List<string> prerequisiteSubjects = preParContainer.PrerequisiteSubjects;
            List<string> parallelSubjects = preParContainer.ParallelSubjects;
            IEnumerable<string> sPreParSubjects = prerequisiteSubjects.Concat(parallelSubjects);
            List<PreParSubject> preParSubjects = new();

            foreach (string strPreParSubject in sPreParSubjects)
            {
                PreParSubject preParSubject = new()
                {
                    SubjectCode = strPreParSubject
                };
                preParSubjects.Add(preParSubject);
            }

            // study state
            StudyState studyState;
            string nodeContent = StringHelper.SuperCleanString(node.InnerText);
            if (nodeContent.Contains("Đã hoàn tất"))
            {
                studyState = StudyState.Completed;
            }
            else
            {
                if (nodeContent.Contains("Chưa có Điểm"))
                {
                    studyState = StudyState.NoHavePoint;
                }
                else
                {
                    studyState = StudyState.UnLearned;
                }
            }

            DataTypes.ProgramSubject subject = new(id, childOfNode, subjectCode, name, studyUnit,
                studyUnitType, prerequisiteSubjects, parallelSubjects, studyState, courseId, "");

            ProgramSubject programSubject = new()
            {
                SubjectCode = subjectCode,
                CourseId = courseId,
                Name = name,
                Credit = studyUnit,
            };

            await _unitOfWork.ProgramSubjects.AddAsync(programSubject);
            foreach (PreParSubject preParSubject in preParSubjects)
            {
                await _unitOfWork.PreParSubjects.AddAsync(preParSubject);
                PreProDetail preProDetail = new()
                {
                    ProgramSubject = programSubject,
                    PreParSubject = preParSubject
                };
                await _unitOfWork.PreProDetails.AddAsync(preProDetail);

                ParProDetail parProDetail = new()
                {
                    ProgramSubject = programSubject,
                    PreParSubject = preParSubject
                };
                await _unitOfWork.ParProDetails.AddAsync(parProDetail);
            }
            await _unitOfWork.CompleteAsync();
            return subject;
        }

        private async Task<List<DataTypes.ProgramSubject>> GetProgramSubjects(IEnumerable<HtmlNode> fileNodes)
        {
            List<DataTypes.ProgramSubject> programSubjects = new();
            foreach (HtmlNode node in fileNodes)
            {
                HtmlDocument doc = new();
                doc.LoadHtml(node.InnerHtml);
                HtmlNode docNode = doc.DocumentNode;

                // id and child of node
                string idValue = node.Attributes["id"].Value;
                string[] classSlices = StringHelper.SplitAndRemoveAllSpace(node.Attributes["class"].Value);
                string classValue = classSlices[0];
                string childOfNode;
                string id = idValue.Split(new char[] { '-' })[1];
                if (classValue.Equals("toptitle", StringComparison.Ordinal))
                {
                    childOfNode = "0";
                }
                else
                {
                    childOfNode = classValue.Split(new char[] { '-' })[3];
                }

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
                int studyUnit = int.Parse(StringHelper.SuperCleanString(tdStudyUnitTag.InnerText));

                //studyunit type
                HtmlNode tdStudyUnitTypeTag = docNode.SelectSingleNode("//td[4]");
                string studyUnitTypeString = StringHelper.SuperCleanString(tdStudyUnitTypeTag.InnerText);
                StudyUnitType studyUnitType = BasicDataConverter.ToStudyUnitType(studyUnitTypeString);

                //prerequisiteSubjects and parallelSubjects
                PreParContainer preParContainer;
                if (_sessionId != null && _sessionId != string.Empty)
                {
                    preParContainer = await _preParSubjectCrawler.Run(courseId, _sessionId);
                }
                else
                {
                    preParContainer = await _preParSubjectCrawler.Run(_sessionId);
                }
                List<string> prerequisiteSubjects = preParContainer.PrerequisiteSubjects;
                List<string> parallelSubjects = preParContainer.ParallelSubjects;
                IEnumerable<string> sPreParSubjects = prerequisiteSubjects.Concat(parallelSubjects);
                List<PreParSubject> preParSubjects = new();

                foreach (string strPreParSubject in sPreParSubjects)
                {
                    PreParSubject preParSubject = new()
                    {
                        SubjectCode = strPreParSubject
                    };
                    preParSubjects.Add(preParSubject);
                }

                // study state
                StudyState studyState;
                string nodeContent = StringHelper.SuperCleanString(node.InnerText);
                if (nodeContent.Contains("Đã hoàn tất"))
                {
                    studyState = StudyState.Completed;
                }
                else
                {
                    if (nodeContent.Contains("Chưa có Điểm"))
                    {
                        studyState = StudyState.NoHavePoint;
                    }
                    else
                    {
                        studyState = StudyState.UnLearned;
                    }
                }

                DataTypes.ProgramSubject subject = new(id, childOfNode, subjectCode, name, studyUnit,
                    studyUnitType, prerequisiteSubjects, parallelSubjects, studyState, courseId, parrentNodeName);
                
                programSubjects.Add(subject);
                
                ProgramSubject programSubject = new()
                {
                    SubjectCode = subjectCode,
                    CourseId = courseId,
                    Name = name,
                    Credit = studyUnit,
                };

                await _unitOfWork.ProgramSubjects.AddAsync(programSubject);
                foreach (PreParSubject preParSubject in preParSubjects)
                {
                    await _unitOfWork.PreParSubjects.AddAsync(preParSubject);
                    PreProDetail preProDetail = new()
                    {
                        ProgramSubject = programSubject,
                        PreParSubject = preParSubject
                    };
                    await _unitOfWork.PreProDetails.AddAsync(preProDetail);

                    ParProDetail parProDetail = new()
                    {
                        ProgramSubject = programSubject,
                        PreParSubject = preParSubject
                    };
                    await _unitOfWork.ParProDetails.AddAsync(parProDetail);
                }
            }
            await _unitOfWork.CompleteAsync();
            return programSubjects;
        }

        private string GetParrentNodeWithId(string id)
        {
            foreach (ProgramFolder programFolder in ProgramFolders)
            {
                if (programFolder.Id.Equals(id, StringComparison.Ordinal))
                {
                    return programFolder.Name;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Trả về tên của một folderNode.
        /// </summary>
        /// <param name="folderNode">Một html folder node.</param>
        /// <returns></returns>
        private static string GetNameFolderNode(HtmlNode folderNode)
        {
            string html = folderNode.InnerHtml;
            HtmlDocument doc = new();
            doc.LoadHtml(html);
            HtmlNode docNode = doc.DocumentNode;
            HtmlNode nodesToRemove = docNode.SelectSingleNode("//span/span");
            nodesToRemove?.Remove();
            HtmlNode span = docNode.SelectSingleNode("//span");
            return StringHelper.SuperCleanString(span.InnerText);
        }

        private static List<ProgramFolder> GetChildFolderNodes(ProgramFolder parrent, List<ProgramFolder> programNodes)
        {
            List<ProgramFolder> childProgramNodes = new();
            foreach (ProgramFolder node in programNodes)
            {
                if (node.GetChildOfNode() == parrent.GetIdNode())
                {
                    childProgramNodes.Add(node);
                }
            }
            return childProgramNodes;
        }

        private static IEnumerable<HtmlNode> GetChildOfNode(string nodeId, IEnumerable<HtmlNode> htmlNodes)
        {
            string classValue = $"child-of-{nodeId}";
            return htmlNodes.Where(node => node.HasClass(classValue));
        }
    }
}
