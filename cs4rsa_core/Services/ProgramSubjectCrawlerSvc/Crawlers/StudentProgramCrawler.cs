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
        private HtmlNodeCollection _trNodes;
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

        public async Task<ProgramFolder> GetNodeA(
            string specialString,
            string t,
            string nodeName,
            int curid
        )
        {
            string url = GetUrl(specialString, t, nodeName, curid);
            _trNodes = await GetAllTrTag(url, nodeName, true);
            ProgramFolder programFolder = GetProgramFolder(_trNodes[0]);
            Root = await GetFolderNode(1, programFolder);
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
        private static string GetUrl(
            string specialString,
            string t,
            string nodeName,
            int curid
        )
        {
            return $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/GetUrl.aspx?t={t}&studentidnumber={specialString}&acaLevid=3&curid={curid}&cursectionid={nodeName}";
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

        private async Task<ProgramFolder> GetFolderNode(int index, ProgramFolder parentFolderNode)
        {
            for (int i = index; i < _trNodes.Count; i++)
            {
                string classValueA = $"child-of-node-{parentFolderNode.Id}";
                if (_trNodes[i].Attributes["class"].Value.Contains(classValueA))
                {
                    HtmlNode node = _trNodes[i];
                    HtmlDocument htmlDocument = new();
                    htmlDocument.LoadHtml(node.InnerHtml);
                    HtmlNode rootNode = htmlDocument.DocumentNode;

                    HtmlNode spanFolderNode = rootNode.SelectSingleNode("//span[@class='folder']");
                    if (spanFolderNode != null && spanFolderNode.Attributes["class"].Value == "folder")
                    {
                        ProgramFolder programFolder = GetProgramFolder(node);
                        parentFolderNode.ChildProgramFolders.Add(await GetFolderNode(i + 1, programFolder));
                        continue;
                    }
                    else if (rootNode.SelectSingleNode("//span[@class='file']") != null)
                    {
                        parentFolderNode.ChildProgramSubjects.Add(await GetProgramSubject(node));
                    }
                }
            }

            return parentFolderNode;
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
    }
}
