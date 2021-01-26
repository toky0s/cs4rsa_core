using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace cs4rsa.BasicData
{
    class Subject
    {
        private string name;
        private string subjectCode;
        private string studyUnit;
        private string studyUnitType;
        private string semester;
        private List<string> mustStudySubject;
        private List<string> parallelSubject;
        private string description;

        private string rawSoup;

        public Subject(string name, string subjectCode, string studyUnit, 
            string studyUnitType, string semester, List<string> mustStudySubject, List<string> parallelSubject,
            string description, string rawSoup)
        {
            this.name = name;
            this.subjectCode = subjectCode;
            this.studyUnit = studyUnit;
            this.studyUnitType = studyUnitType;
            this.semester = semester;
            this.mustStudySubject = mustStudySubject;
            this.parallelSubject = parallelSubject;
            this.description = description;
            this.rawSoup = rawSoup;
        }

        public List<ClassGroup> getClassGroups()
        {
            return null;
        }
    }
}
