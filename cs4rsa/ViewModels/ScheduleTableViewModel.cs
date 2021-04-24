using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using cs4rsa.BaseClasses;
using cs4rsa.BasicData;
using cs4rsa.Models;

namespace cs4rsa.ViewModels
{
    class ScheduleTableViewModel: NotifyPropertyChangedBase
    {
        private List<ClassGroupModel> classGroupModels = new List<ClassGroupModel>();

        private List<ClassGroupModel> Phase1 = new List<ClassGroupModel>();
        private List<ClassGroupModel> Phase2 = new List<ClassGroupModel>();

        public ScheduleTableViewModel()
        {

        }

        private void DivideClassGroupsByPhases()
        {
            foreach(ClassGroupModel classGroupModel in classGroupModels)
            {
                if (classGroupModel.Phase == Phase.FIRST)
                    Phase1.Add(classGroupModel);
                if (classGroupModel.Phase == Phase.SECOND)
                    Phase2.Add(classGroupModel);
                if (classGroupModel.Phase == Phase.ALL)
                    Phase1.Add(classGroupModel);
                    Phase2.Add(classGroupModel);
            }
        }

        private List<string> GetTimeString(List<ClassGroupModel> classGroupModels)
        {
            return null;
        }
    }
}
