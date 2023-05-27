using Cs4rsa.BaseClasses;
using Cs4rsa.Cs4rsaDatabase.Interfaces;
using Cs4rsa.Cs4rsaDatabase.Models;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.ProgramSubjectCrawlerSvc.Interfaces;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes.Enums;
using Cs4rsa.Services.SubjectCrawlerSvc.Utils;
using Cs4rsa.Utils;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cs4rsa.Services.ProgramSubjectCrawlerSvc.Crawlers
{
    public class StudentProgramCrawler : BaseCrawler, IStudentProgramCrawler
    {
        private static string _specialString;
        private static int _curid;
        /// <summary>
        /// Có thể thực hiện lưu DbProgramSubject hay không.
        /// </summary>
        private static bool _canSaveSubject;

        private HtmlNodeCollection _trNodes2001;
        private HtmlNodeCollection _trNodes2002;
        private HtmlNodeCollection _trNodes2003;
        private HtmlNodeCollection _trNodes2004;

        private readonly HtmlWeb _htmlWeb;
        private readonly IUnitOfWork _unitOfWork;

        public StudentProgramCrawler(
            HtmlWeb htmlWeb,
            IUnitOfWork unitOfWork
        )
        {
            _htmlWeb = htmlWeb;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy ra đường dẫn tới phần học
        /// </summary>
        /// <param name="specialString">Mã đặc biệt</param>
        /// <param name="nodeName">PhanHoc</param>
        /// <param name="curid">Mã ngành</param>
        /// <returns></returns>
        private static string GetUrl(string nodeName)
        {
            return $"https://mydtu.duytan.edu.vn/Modules/curriculuminportal/ajax/LoadChuongTrinhHocEachPart.aspx?t={GetTimeFromEpoch()}&studentidnumber={_specialString}&acaLevid=3&curid={_curid}&cursectionid={nodeName}";
        }

        private async Task<HtmlNodeCollection> GetAllTrTag(string url)
        {
            HtmlDocument doc = await _htmlWeb.LoadFromWebAsync(url);
            HtmlNodeCollection trTags = doc.DocumentNode.SelectNodes("//tr");
            trTags.RemoveAt(0);
            return trTags;
        }

        /// <summary>
        /// GetByPaging Folder Node
        /// 
        /// Đệ quy sâu lấy thông tin của folder node.
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="parentFolderNode">Folder Node tham chiếu</param>
        /// <returns>Folder Node và các child của nó.</returns>
        private async Task<ProgramFolder> GetFolderNode(
            int index,
            ProgramFolder parentFolderNode,
            HtmlNodeCollection trNodeCollection,
            List<Task> preparTasks)
        {
            for (int i = index; i < trNodeCollection.Count; i++)
            {
                if (trNodeCollection[i].Attributes["class"].Value.Split(' ').First().Equals($"child-of-node-{parentFolderNode.Id}"))
                {
                    HtmlNode node = trNodeCollection[i];
                    HtmlDocument htmlDocument = new();
                    htmlDocument.LoadHtml(node.InnerHtml);
                    HtmlNode rootNode = htmlDocument.DocumentNode;

                    HtmlNode spanFolderNode = rootNode.SelectSingleNode("//span[@class='folder']");
                    if (spanFolderNode != null && spanFolderNode.Attributes["class"].Value == "folder")
                    {
                        ProgramFolder programFolder = GetProgramFolder(node);
                        await GetFolderNode(i + 1, programFolder, trNodeCollection, preparTasks);
                        parentFolderNode.ChildProgramFolders.Add(programFolder);
                    }
                    else
                    {
                        parentFolderNode.ChildProgramSubjects.Add(
                            GetProgramSubject(node, parentFolderNode, preparTasks)
                        );
                    }
                }
            }
            parentFolderNode.Completed = parentFolderNode.IsCompleted();
            return parentFolderNode;
        }

        /// <summary>
        /// GetByPaging Program Folder
        /// 
        /// Tạo mới một instance Program Folder từ một Html trNode.
        /// </summary>
        /// <param name="trNode">tr Html Node.</param>
        /// <returns>ProgramFolder</returns>
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

        /// <summary>
        /// GetByPaging Program Subject
        /// 
        /// Lấy ra một Program Subject từ một tr node.
        /// </summary>
        /// <param name="node">tr Html Node.</param>
        /// <param name="parentNode">Program Folder cha chứa Program Subject này.</param>
        /// <returns>ProgramSubject</returns>
        private ProgramSubject GetProgramSubject(HtmlNode node, ProgramFolder parentNode, List<Task> preparTasks)
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

            ProgramSubject subject = new(
                id,
                childOfNode,
                subjectCode,
                name,
                studyUnit,
                studyUnitType,
                studyState,
                courseId,
                parrentNodeName
            );

            if (_canSaveSubject)
            {
                DbProgramSubject dbProgramSubject = new()
                {
                    SubjectCode = subjectCode,
                    CourseId = courseId,
                    Name = name,
                    Credit = studyUnit,
                    CurriculumId = _curid
                };

                _unitOfWork.ProgramSubjects.Add(dbProgramSubject);
            }

            return subject;
        }

        /// <summary>
        /// Trả về tên của một folderNode.
        /// </summary>
        /// <param name="folderNode">Một html folder node.</param>
        /// <returns>Tên của Folder node.</returns>
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

        /// <summary>
        /// Set thông tin trước khi thực hiện lấy chương trình học.
        /// </summary>
        /// <param name="specialString">Special String của mỗi sinh viên</param>
        /// <param name="curid">Mã ngành</param>
        public async Task<ProgramFolder[]> GetProgramFolders(string specialString, int curid)
        {
            const string NODE_DAI_CUONG = "2001";
            const string NODE_GDTC_VA_QP = "2002";
            const string NODE_DAI_CUONG_NGANH = "2003";
            const string NODE_CHUYEN_NGANH = "2004";

            _specialString = specialString;
            _curid = curid;
            _canSaveSubject = !_unitOfWork.Curriculums.ExistsById(curid);

            string url2001 = GetUrl(NODE_DAI_CUONG);
            string url2002 = GetUrl(NODE_GDTC_VA_QP);
            string url2003 = GetUrl(NODE_DAI_CUONG_NGANH);
            string url2004 = GetUrl(NODE_CHUYEN_NGANH);

            HtmlNodeCollection[] cols = await Task.WhenAll(
                GetAllTrTag(url2001),
                GetAllTrTag(url2002),
                GetAllTrTag(url2003),
                GetAllTrTag(url2004)
            );

            _trNodes2001 = cols[0];
            _trNodes2002 = cols[1];
            _trNodes2003 = cols[2];
            _trNodes2004 = cols[3];

            ProgramFolder pf2001 = GetProgramFolder(_trNodes2001[0]);
            ProgramFolder pf2002 = GetProgramFolder(_trNodes2002[0]);
            ProgramFolder pf2003 = GetProgramFolder(_trNodes2003[0]);
            ProgramFolder pf2004 = GetProgramFolder(_trNodes2004[0]);

            List<Task> preparTasks2001 = new();
            List<Task> preparTasks2002 = new();
            List<Task> preparTasks2003 = new();
            List<Task> preparTasks2004 = new();

            ProgramFolder[] programFolders = await Task.WhenAll(
                GetFolderNode(1, pf2001, _trNodes2001, preparTasks2001),
                GetFolderNode(1, pf2002, _trNodes2002, preparTasks2002),
                GetFolderNode(1, pf2003, _trNodes2003, preparTasks2003),
                GetFolderNode(1, pf2004, _trNodes2004, preparTasks2004)
            );

            // Thêm task cào data của môn tiên quyết và song hành
            await Task.WhenAll(
                preparTasks2001
                .Concat(preparTasks2002)
                .Concat(preparTasks2003)
                .Concat(preparTasks2004)
                .ToList()
            );

            return programFolders;
        }
    }
}
