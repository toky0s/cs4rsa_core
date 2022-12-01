using Cs4rsa.Services.SubjectCrawlerSvc.Models;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cs4rsa.ViewModels.Interfaces
{
    internal interface IPhaseStore
    {
        public ObservableCollection<int> Weeks { get;}
        public int BetweenPoint { get; }

        public void AddClassGroupModel(ClassGroupModel classGroupModel);
        public void AddClassGroupModels(IEnumerable<ClassGroupModel> classGroupModels);

        public void RemoveClassGroup(ClassGroupModel classGroupModel);
        public void RemoveClassGroup(SubjectModel subjectModel);
        public void RemoveClassGroups(IEnumerable<ClassGroupModel> classGroupModels);
        public void RemoveAll();

        public void EvaluateBetweenPoint();
    }
}
