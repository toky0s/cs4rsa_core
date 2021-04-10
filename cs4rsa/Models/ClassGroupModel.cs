using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs4rsa.BasicData;

namespace cs4rsa.Models
{
    public class ClassGroupModel
    {
        private ClassGroup classGroup;
        public string Name
        {
            get
            {
                return classGroup.Name;
            }
        }

        public string SubjectCode
        {
            get
            {
                return classGroup.SubjectCode;
            }
        }

        public ClassGroupModel(ClassGroup classGroup)
        {
            this.classGroup = classGroup;
        }
    }
}
