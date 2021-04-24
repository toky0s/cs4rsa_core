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

        public string Name => classGroup.Name;
        public string SubjectCode => classGroup.SubjectCode;
        public Phase Phase => classGroup.GetPhase();
        public Schedule Schedule => classGroup.GetSchedule();

        public ClassGroupModel(ClassGroup classGroup)
        {
            this.classGroup = classGroup;
        }
    }
}
