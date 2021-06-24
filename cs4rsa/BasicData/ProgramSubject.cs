using System.Collections.Generic;

namespace cs4rsa.BasicData
{
    /// <summary>
    /// Đại diện cho tình trạng học của một môn như Đã hoàn tất, 
    /// Đã đang học-Chưa có điểm, hoặc các môn chưa học (và chưa đăng ký)
    /// </summary>
    public enum StudyState
    {
        Completed,
        NoHavePoint,
        UnLearned
    }

    /// <summary>
    /// Đại diện cho một Row của trong bảng chương trình học.
    /// Chứa thông tin cơ bản của Môn có trong chương trình.
    /// </summary>
    public class ProgramSubject
    {
        private string _subjectCode;
        private string _subjectName;
        private string _studyUnit;
        private string _studyUnitType;
        // Lần lượt là các môn tiên quyết và song hành
        private List<ProgramSubject> _prerequisiteSubjects;
        private List<ProgramSubject> _parallelSubject;
        private StudyState _studyState;

        public ProgramSubject(string subjectCode, string subjectName, string studyUnit, string studyUnitType,
            List<ProgramSubject> prerequisiteSubjects, List<ProgramSubject> parallelSubject, StudyState studyState)
        {
            _subjectCode = subjectCode;
            _subjectName = subjectName;
            _studyUnit = studyUnit;
            _studyUnitType = studyUnitType;
            _prerequisiteSubjects = prerequisiteSubjects;
            _parallelSubject = parallelSubject;
            _studyState = studyState;
        }
    }
}