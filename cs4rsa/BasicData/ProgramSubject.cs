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

        public string Id => _id;
        public string ChildOfNode => _childOfNode;
        public string SubjectCode => _subjectCode;
        public string SubjectName => _subjectName;
        public string StudyUnit => _studyUnit;
        public StudyUnitType StudyUnitType => _studyUnitType;
        public List<string> PrerequisiteSubjects => _prerequisiteSubjects;
        public List<string> ParallelSubjects => _parallelSubjects;
        public StudyState StudyState => _studyState;

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

        public ProgramSubject(ProgramSubject programSubject)
        {
            _id = programSubject.Id;
            _childOfNode = programSubject.ChildOfNode;
            _subjectCode = programSubject.SubjectCode;
            _subjectName = programSubject.SubjectName;
            _studyUnit = programSubject.StudyUnit;
            _studyUnitType = programSubject.StudyUnitType;
            _prerequisiteSubjects = programSubject.PrerequisiteSubjects;
            _parallelSubjects = programSubject.ParallelSubjects;
            _studyState = programSubject.StudyState;
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
            if (_studyState == StudyState.Completed)
                return true;
            return false;
        }

        public bool IsUnLearn()
        {
            if (_studyState == StudyState.UnLearned)
                return true;
            return false;
        }
    }
}