using cs4rsa.Enums;
using cs4rsa.Interfaces;
using System;
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
    public class ProgramSubject: IProgramNode, IComparable
    {
        private string _id;
        private readonly string _childOfNode;
        private string _subjectCode;
        private string _subjectName;
        private readonly string _studyUnit;
        private readonly StudyUnitType _studyUnitType;
        // Lần lượt là các môn tiên quyết và song hành
        private readonly List<string> _prerequisiteSubjects;
        private readonly List<string> _parallelSubjects;
        private readonly StudyState _studyState;
        private readonly string _courseId;
        private readonly string _parrentNodeName;

        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public string ChildOfNode => _childOfNode;
        public string SubjectCode
        {
            get
            {
                return _subjectCode;
            }
            set
            {
                _subjectCode = value;
            }
        }
        public string SubjectName
        {
            get
            {
                return _subjectName;
            }
            set
            {
                _subjectName = value;
            }
        }
        public int StudyUnit => int.Parse(_studyUnit);
        public StudyUnitType StudyUnitType => _studyUnitType;
        public List<string> PrerequisiteSubjects => _prerequisiteSubjects;
        public List<string> ParallelSubjects => _parallelSubjects;
        public StudyState StudyState => _studyState;
        public string CourseId => _courseId;
        public string ParrentNodeName => _parrentNodeName;

        public ProgramSubject(string id, string childOfNode, string subjectCode, string subjectName, string studyUnit, StudyUnitType studyUnitType,
            List<string> prerequisiteSubjects, List<string> parallelSubject, StudyState studyState, string courseId, string parrentNodeName)
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
            _courseId = courseId;
            _parrentNodeName = parrentNodeName;
        }

        public string GetChildOfNode()
        {
            return _childOfNode;
        }

        public string GetIdNode()
        {
            return _id;
        }
        
        /// <summary>
        /// Kiểm tra xem môn học này đã học qua hay chưa.
        /// </summary>
        /// <returns></returns>
        public bool IsDone()
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

        public int CompareTo(object obj)
        {
            ProgramSubject other = obj as ProgramSubject;
            return Id.CompareTo(other.Id);
        }

        public NodeType GetNodeType()
        {
            return NodeType.Subject;
        }
    }
}