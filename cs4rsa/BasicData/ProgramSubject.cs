using cs4rsa.Enums;
using cs4rsa.Interfaces;
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
    public class ProgramSubject: IProgramNode
    {
        private string _id;
        private string _childOfNode;
        private string _subjectCode;
        private string _subjectName;
        private string _studyUnit;
        private StudyUnitType _studyUnitType;
        // Lần lượt là các môn tiên quyết và song hành
        private List<string> _prerequisiteSubjects;
        private List<string> _parallelSubjects;
        private StudyState _studyState;

        public ProgramSubject(string id, string childOfNode, string subjectCode, string subjectName, string studyUnit, StudyUnitType studyUnitType,
            List<string> prerequisiteSubjects, List<string> parallelSubject, StudyState studyState)
        {
            _id = id;
            _childOfNode = childOfNode;
            _subjectCode = subjectCode;
            _subjectName = subjectName;
            _studyUnit = studyUnit;
            _studyUnitType = studyUnitType;
            _prerequisiteSubjects = prerequisiteSubjects;
            _parallelSubjects = parallelSubject;
            _studyState = studyState;
        }

        public string GetChildOfNode()
        {
            return _childOfNode;
        }

        public string GetIdNode()
        {
            return _id;
        }

        public bool IsCompleted()
        {
            if (_studyState != StudyState.UnLearned)
                return true;
            return false;
        }
    }
}