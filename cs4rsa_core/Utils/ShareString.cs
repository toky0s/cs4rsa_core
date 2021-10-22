using CourseSearchService.Crawlers.Interfaces;
using Cs4rsaDatabaseService.DataProviders;
using cs4rsa_core.Dialogs.DialogResults;
using SubjectCrawlService1.DataTypes;
using System.Collections.Generic;
using System.Linq;
using Cs4rsaDatabaseService.Models;
using Cs4rsaDatabaseService.Interfaces;

namespace cs4rsa_core.Utils
{
    /// <summary>
    /// ShareString là một tính năng quan trọng giúp các
    /// sinh viên chia sẻ lịch học mà họ đã sắp xếp thông qua
    /// các phương tiện truyền thông, giúp giảm thời gian lựa chọn
    /// môn học cũng như nhóm lớp.
    /// </summary>
    public class ShareString
    {
        private IUnitOfWork _unitOfWork;
        private ICourseCrawler _courseCrawler;
        public ShareString(IUnitOfWork unitOfWork, ICourseCrawler courseCrawler)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
        }
        public string GetShareString(List<ClassGroup> classGroupModels)
        {
            List<string> SubjectHasses = classGroupModels.Select(item => SubjectCodeVsRegisterCode(item)).ToList();
            string SubjectHassesJoin = string.Join("?", SubjectHasses);
            string Count = classGroupModels.Count.ToString();
            string currentYear = _courseCrawler.GetCurrentYearValue();
            string currentSemester = _courseCrawler.GetCurrentSemesterValue();
            return $"cs4rsa!{currentYear}!{currentSemester}!{Count}!{SubjectHassesJoin}".Replace(' ','-');
        }

        public SessionManagerResult GetSubjectFromShareString(string shareString)
        {
            // cs4rsa!69!73!2!%ACC 201|ACC201202103003%?%ACC 411|ACC411202103001%
            string[] shareStringSlices = shareString.Split(new char[] { '!' });
            string currentYear = shareStringSlices[1];
            string currentSemester = shareStringSlices[2];
            int count = int.Parse(shareStringSlices[3]);

            // ACC-201|ACC201202103003?ACC-411|ACC411202103001
            string subjectHasses = shareStringSlices[4].Replace('-',' ');
            string[] subjectHassesSlices = subjectHasses.Split(new char[] { '?' });
            List<SubjectInfoData> subjectInfoDatas = new List<SubjectInfoData>();
            foreach(string item in subjectHassesSlices)
            {
                string[] infoes = item.Split(new char[] { '|' });
                string subjectCode = infoes[0];
                string classGroupName = infoes[1];

                string discipline = subjectCode.Split(new char[] { ' ' })[0];
                string keyword1 = subjectCode.Split(new char[] { ' ' })[1];
                Keyword keyword = _unitOfWork.Keywords.GetKeyword(discipline, keyword1);
                string subjectName = keyword.SubjectName;
                SubjectInfoData subjectInfoData = new SubjectInfoData()
                {
                    SubjectCode = subjectCode,
                    ClassGroup = classGroupName,
                    SubjectName = subjectName
                };
                subjectInfoDatas.Add(subjectInfoData);
            }
            return new SessionManagerResult(subjectInfoDatas);
        }

        private static string SubjectCodeVsRegisterCode(ClassGroup classGroup)
        {
            return classGroup.SubjectCode + "|" + classGroup.Name;
        }
    }
}
