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

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers
{
    public class StudentProgramCrawler
    {
        private static string _studentId;
        private static bool _isUseCache;

        private HtmlNodeCollection _trNodes;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPreParSubjectCrawler _preParSubjectCrawler;

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
        /// <param name="isUseCache">
        /// True nếu sử dụng thông tin được lưu từ DB. 
        /// Ngược lại sẽ thực hiện cào lại thông tin mới.
        /// </param>
        /// <param name="studentId">Student ID</param>
        public static void SetInfor(string studentId, bool isUseCache)
        {
            _studentId = studentId;
            _isUseCache = isUseCache;
        }

        public async Task<ProgramFolder> GetRoot(
            string specialString,
            string t,
            string nodeName,
            int curid
        )
        {
            string url = GetUrl(specialString, t, nodeName, curid);
            _trNodes = await GetAllTrTag(url, nodeName);
            ProgramFolder programFolder = GetProgramFolder(_trNodes[0]);
            return await GetFolderNode(1, programFolder);
        }

        /// <summary>
        /// Lấy ra đường dẫn tới phần học
        /// </summary>
        /// <param name="specialString">Mã đặc biệt</param>
        /// <param name="t">Epoch</param>
        /// <param name="curid">Mã ngành</param>
        /// <param name="cursectionid">PhanHoc</param>
        /// <returns></returns>
        private static string GetUrl(string specialString, string t, string nodeName, int curid)
        {
            return $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={t}&studentidnumber={specialString}&acaLevid=3&curid={curid}&cursectionid={nodeName}";
        }

        private static async Task<HtmlNodeCollection> GetAllTrTag(string url, string nodeName)
        {
            HtmlWeb web = new();
            HtmlDocument doc = new();
            string path = Path.Combine(AppContext.BaseDirectory, IFolderManager.FD_STUDENT_PROGRAMS, _studentId, nodeName + ".html");
            if (File.Exists(path) && _isUseCache)
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
                if (_trNodes[i].Attributes["class"].Value.Split(' ').First().Equals($"child-of-node-{parentFolderNode.Id}"))
                {
                    HtmlNode node = _trNodes[i];
                    HtmlDocument htmlDocument = new();
                    htmlDocument.LoadHtml(node.InnerHtml);
                    HtmlNode rootNode = htmlDocument.DocumentNode;

                    HtmlNode spanFolderNode = rootNode.SelectSingleNode("//span[@class='folder']");
                    if (spanFolderNode != null && spanFolderNode.Attributes["class"].Value == "folder")
                    {
                        ProgramFolder programFolder = GetProgramFolder(node);
                        await GetFolderNode(i + 1, programFolder);
                        parentFolderNode.ChildProgramFolders.Add(programFolder);
                    }
                    else
                    {
                        parentFolderNode.ChildProgramSubjects.Add(await GetProgramSubject(node, parentFolderNode));
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

        private async Task<ProgramSubject> GetProgramSubject(HtmlNode node, ProgramFolder parentNode)
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

            string parrentNodeName = parentNode.Name;

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

            //prerequisiteSubjects and parallelSubjects
            IEnumerable<string> prerequisiteSubjects;
            IEnumerable<string> parallelSubjects;
            // TODO: Slow here
            PreParContainer preParContainer = await _preParSubjectCrawler.Run(courseId, _isUseCache);
            if (preParContainer != null)
            {
                prerequisiteSubjects = preParContainer.PrerequisiteSubjects;
                parallelSubjects = preParContainer.ParallelSubjects;
            }
            else
            {
                prerequisiteSubjects = new List<string>();
                parallelSubjects = new List<string>();
            }

            ProgramSubject subject = new(
                id,
                childOfNode,
                subjectCode,
                name,
                studyUnit,
                studyUnitType,
                prerequisiteSubjects,
                parallelSubjects,
                studyState,
                courseId,
                parrentNodeName
            );

            DbProgramSubject programSubject = new()
            {
                SubjectCode = subjectCode,
                CourseId = courseId,
                Name = name,
                Credit = studyUnit,
            };
            await _unitOfWork.ProgramSubjects.AddAsync(programSubject);

            IEnumerable<DbPreParSubject> preParSubjects = GetPreParSubjects(prerequisiteSubjects.Concat(parallelSubjects));
            foreach (DbPreParSubject preParSubject in preParSubjects)
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

        private static IEnumerable<DbPreParSubject> GetPreParSubjects(IEnumerable<string> preParSubjectNames)
        {
            foreach (string strPreParSubject in preParSubjectNames)
            {
                yield return new() { SubjectCode = strPreParSubject };
            }
        }
    }
}
