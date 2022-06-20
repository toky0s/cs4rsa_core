using CourseSearchService.Crawlers.Interfaces;
using cs4rsa_core.Dialogs.DialogResults;
using Cs4rsaDatabaseService.Interfaces;
using Cs4rsaDatabaseService.Models;
using HelperService;
using SubjectCrawlService1.DataTypes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
        #region DI
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICourseCrawler _courseCrawler;
        #endregion

        public ShareString(IUnitOfWork unitOfWork, ICourseCrawler courseCrawler)
        {
            _unitOfWork = unitOfWork;
            _courseCrawler = courseCrawler;
        }
        public string GetShareString(List<ClassGroup> classGroupModels)
        {
            if (classGroupModels.Count == 0)
            {
                return "";
            }
            List<string> SubjectHasses = classGroupModels.Select(item => SubjectCodeVsRegisterCode(item)).ToList();
            string SubjectHassesJoin = string.Join("?", SubjectHasses);
            string Count = classGroupModels.Count.ToString();
            string currentYear = _courseCrawler.GetCurrentYearValue();
            string currentSemester = _courseCrawler.GetCurrentSemesterValue();
            string raw = $"cs4rsa!{currentYear}!{currentSemester}!{Count}!{SubjectHassesJoin}".Replace(' ', '-');
            return StringHelper.EncodeTo64(raw);
        }

        public SessionManagerResult GetSubjectFromShareString(string shareString)
        {
            try
            {
                shareString = StringHelper.DecodeFrom64(shareString);
                string[] shareStringSlices = shareString.Split(new char[] { '!' });

                string subjectHasses = shareStringSlices[4].Replace('-', ' ');
                string[] subjectHassesSlices = subjectHasses.Split(new char[] { '?' });
                List<SubjectInfoData> subjectInfoDatas = new();
                foreach (string item in subjectHassesSlices)
                {
                    string[] infoes = item.Split(new char[] { '|' });
                    string subjectCode = infoes[0];
                    string classGroupName = infoes[1];
                    string registerCode = infoes[2];

                    string discipline = subjectCode.Split(new char[] { ' ' })[0];
                    string keyword1 = subjectCode.Split(new char[] { ' ' })[1];
                    Keyword keyword = _unitOfWork.Keywords.GetKeyword(discipline, keyword1);
                    string subjectName = keyword.SubjectName;
                    SubjectInfoData subjectInfoData = new()
                    {
                        SubjectCode = subjectCode,
                        ClassGroup = classGroupName,
                        SubjectName = subjectName,
                        RegisterCode = registerCode
                    };
                    subjectInfoDatas.Add(subjectInfoData);
                }
                return new SessionManagerResult(subjectInfoDatas);
            }
            catch
            {
                MessageBox.Show("ShareString có vấn đề 🤔");
                return null;
            }
        }

        private static string SubjectCodeVsRegisterCode(ClassGroup classGroup)
        {
            return classGroup.SubjectCode + "|" + classGroup.Name + "|" + classGroup.GetRegisterCode();
        }
    }
}
