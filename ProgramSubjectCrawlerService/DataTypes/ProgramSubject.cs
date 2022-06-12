using ProgramSubjectCrawlerService.DataTypes.Enums;
using ProgramSubjectCrawlerService.DataTypes.Interfaces;

using SubjectCrawlService1.DataTypes.Enums;

using System;
using System.Collections.Generic;

namespace ProgramSubjectCrawlerService.DataTypes
{
    /// <summary>
    /// Đại diện cho một Row của trong bảng chương trình học.
    /// Chứa thông tin cơ bản của Môn có trong chương trình.
    /// </summary>
    public class ProgramSubject : IProgramNode, IComparable
    {
        public string Id { get; set; }
        public string ChildOfNode { get; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public byte StudyUnit { get; }
        public StudyUnitType StudyUnitType { get; }
        public List<string> PrerequisiteSubjects { get; }
        public List<string> ParallelSubjects { get; }
        public StudyState StudyState { get; }
        public string CourseId { get; }
        public string ParentNodeName { get; }

        public ProgramSubject(string id, string childOfNode, string subjectCode, string subjectName, byte studyUnit, StudyUnitType studyUnitType,
            List<string> prerequisiteSubjects, List<string> parallelSubject, StudyState studyState, string courseId, string parrentNodeName)
        {
            Id = id;
            ChildOfNode = childOfNode;
            SubjectCode = subjectCode;
            SubjectName = subjectName;
            StudyUnit = studyUnit;
            StudyUnitType = studyUnitType;
            PrerequisiteSubjects = prerequisiteSubjects;
            ParallelSubjects = parallelSubject;
            StudyState = studyState;
            CourseId = courseId;
            ParentNodeName = parrentNodeName;
        }

        public string GetChildOfNode()
        {
            return ChildOfNode;
        }

        public string GetIdNode()
        {
            return Id;
        }

        /// <summary>
        /// Kiểm tra xem môn học này đã học qua hay chưa.
        /// </summary>
        /// <returns>True nếu đã pass, ngược lại trả về false.</returns>
        public bool IsDone()
        {
            if (StudyState == StudyState.Completed)
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