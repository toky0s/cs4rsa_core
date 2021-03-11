using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;


namespace cs4rsa.Models
{
    public class SubjectModel
    {
        private Subject subject;
        private readonly List<ClassGroupModel> classGroupModels;
        public List<ClassGroupModel> ClassGroupModels
        {
            get
            {
                List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();
                foreach(ClassGroup classGroup in subject.GetClassGroups())
                {
                    classGroupModels.Add(new ClassGroupModel(classGroup));
                }
                return classGroupModels;
            }
        }
        public string SubjectName
        {
            get
            {
                return subject.Name;
            }
        }
        public string SubjectCode
        {
            get
            {
                return subject.SubjectCode;
            }
        }

        public SubjectModel(Subject subject)
        {
            this.subject = subject;
        }
    }
}
