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
        private readonly ClassGroup classGroup;

        public readonly string Name;
        public readonly string SubjectCode;
        public readonly Phase Phase;

        public ClassGroupModel(ClassGroup classGroup)
        {
            this.classGroup = classGroup;
            Name = classGroup.Name;
            SubjectCode = classGroup.SubjectCode;
            Phase = classGroup.GetPhase();
        }
    }
}
