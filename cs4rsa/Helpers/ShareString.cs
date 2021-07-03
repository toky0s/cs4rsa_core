using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Models;
using System.Collections.Generic;
using System.Linq;

namespace cs4rsa.Helpers
{
    /// <summary>
    /// ShareString là một tính năng quan trọng giúp các
    /// sinh viên chia sẻ lịch học mà họ đã sắp xếp thông qua
    /// các phương tiện truyền thông, giúp giảm thời gian lựa chọn
    /// môn học cũng như nhóm lớp.
    /// </summary>
    public class ShareString
    {
        public static string GetShareString(List<ClassGroupModel> classGroupModels)
        {
            HomeCourseSearch homeCourseSearch = HomeCourseSearch.GetInstance();
            List<string> SubjectHasses = classGroupModels.Select(item => SubjectCodeVsRegisterCode(item)).ToList();
            string SubjectHassesJoin = string.Join("?", SubjectHasses);
            string Count = classGroupModels.Count.ToString();
            string currentYear = homeCourseSearch.CurrentYearValue;
            string currentSemester = homeCourseSearch.CurrentSemesterValue;
            return $"cs4rsa!{currentYear}!{currentSemester}!{Count}!{SubjectHassesJoin}".Replace(' ','-');
        }

        public static SessionManagerResult GetSubjectFromShareString(string shareString)
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
                string subjectName = Cs4rsaDataView.GetSubjectName(subjectCode);
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

        private static string SubjectCodeVsRegisterCode(ClassGroupModel classGroupModel)
        {
            return classGroupModel.SubjectCode + "|" + classGroupModel.Name;
        }
    }
}
