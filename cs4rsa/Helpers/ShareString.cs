using cs4rsa.Crawler;
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
            string subjectCode = "";
            string registerCode = "";
            string cs4rsaHashCode = "";
            string replaceChar = "%";
            foreach (ClassGroupModel classGroupModel in classGroupModels)
            {
                subjectCode = classGroupModel.SubjectCode;
                registerCode = classGroupModel.RegisterCode;
                cs4rsaHashCode = cs4rsaHashCode + subjectCode + replaceChar + registerCode;
            }
            return StringHelper.SuperCleanString($"#cs4rsa!{homeCourseSearch.CurrentSemesterValue}!{classGroupModels.Count}!{cs4rsaHashCode}#cs4rsa");
        }

        public static SessionManagerResult GetSubjectFromShareString(string shareString)
        {
            return null;
        }
    }
}
