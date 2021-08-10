using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Dialogs.DialogResults;
using cs4rsa.Dialogs.DialogService;
using cs4rsa.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace cs4rsa.Dialogs.Implements
{
    public class AutoSortViewModel : DialogViewModelBase<AutoSortResult>
    {
        private int _progressValue;
        public int ProgressValue
        {
            get
            {
                return _progressValue;
            }
            set
            {
                _progressValue = value;
                OnPropertyChanged();
            }
        }

        private List<ProgramSubjectModel> _programSubjectModels;

        public AutoSortViewModel(List<ProgramSubjectModel> programSubjectModels, bool isRemoveClassGroupInvalid)
        {
            _programSubjectModels = programSubjectModels;
            Sort(isRemoveClassGroupInvalid);
        }

        private void Sort(bool IsRemoveClassGroupInvalid)
        {
            List<string> courseIds = _programSubjectModels.Select(item => item.CourseId).ToList();
            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            Tuple<List<string>, bool> tuplePara = Tuple.Create(courseIds, IsRemoveClassGroupInvalid);
            backgroundWorker.RunWorkerAsync(tuplePara);
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Tuple<List<SubjectModel>, List<List<ClassGroupModel>>> resultOfSorting = e.Result as Tuple<List<SubjectModel>, List<List<ClassGroupModel>>>;
            AutoSortResult result = new AutoSortResult(resultOfSorting.Item1, resultOfSorting.Item2);
            CloseDialogWithResult(result);
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressValue = e.ProgressPercentage;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Tuple<List<string>, bool> para = e.Argument as Tuple<List<string>, bool>;

            List<string> courseIds = para.Item1 as List<string>;
            bool isRemoveClassGroupInvalid = (bool)para.Item2;

            List<Subject> subjects = new List<Subject>();
            worker.ReportProgress(10);
            foreach (string courseId in courseIds)
            {
                SubjectCrawler subjectCrawler = new SubjectCrawler(courseId);
                Subject subject = subjectCrawler.ToSubject();
                subjects.Add(subject);
            }
            worker.ReportProgress(80);
            List<ClassGroupModel> elements = new List<ClassGroupModel>();
            foreach (Subject subject in subjects)
            {
                List<ClassGroupModel> classGroupModelNames = subject.ClassGroups.Select(cl => new ClassGroupModel(cl)).ToList();
                if (isRemoveClassGroupInvalid)
                {
                    classGroupModelNames = classGroupModelNames.Where(item => item.HaveSchedule && item.EmptySeat > 0).ToList();
                }
                elements.AddRange(classGroupModelNames);
            }
            List<SubjectModel> subjectModels = subjects.Select(item => new SubjectModel(item)).ToList();
            List<List<ClassGroupModel>> cases = Gen(subjects.Count, elements);
            e.Result = Tuple.Create<List<SubjectModel>, List<List<ClassGroupModel>>>(subjectModels, cases);
            worker.ReportProgress(100);
        }

        #region Generation Algorithm
        /// <summary>
        /// Sinh tổ hợp chính tắc từ tập class Group
        /// </summary>
        /// <param name="k">Số lượng phần tử trong mỗi tổ hợp.</param>
        /// <param name="elements">Danh sách các phần tử duy nhất.</param>
        /// <returns></returns>
        public static List<List<ClassGroupModel>> Gen(int k, List<ClassGroupModel> elements)
        {
            List<List<ClassGroupModel>> result = new List<List<ClassGroupModel>>();
            List<ClassGroupModel> firstCombination = new List<ClassGroupModel>();
            if (firstCombination.Count > 0)
            {
                for (int i = 0; i < k; i++)
                {
                    firstCombination.Add(elements[i]);
                }
                while (true)
                {
                    result.Add(firstCombination.ToList());
                    if (IsLastCombination(firstCombination, k, elements))
                    {
                        return result;
                    }
                    firstCombination = GenNext(firstCombination, k, elements);
                }
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra tổi hợp có phải là KHÔNG phải là tổ hợp cuối cùng trong tập hợp hay không.
        /// </summary>
        /// <param name="combination">Tổ hợp phần tử.</param>
        /// <param name="k">Số lượng phần tử mỗi tổ hợp (chập).</param>
        /// <param name="elements">Danh sách phần tử.</param>
        /// <returns></returns>
        public static bool IsLastCombination(List<ClassGroupModel> combination, int k, List<ClassGroupModel> elements)
        {
            List<ClassGroupModel> lastCombination = elements.GetRange(elements.Count - k, k);
            if (combination.Count != lastCombination.Count)
                return false;
            int count = 0;
            for (int i = 0; i < k; i++)
            {
                if (combination[i].Equals(lastCombination[i]))
                    count++;
            }
            if (count == lastCombination.Count)
                return true;
            return false;
        }

        /// <summary>
        /// Sinh tổ hợp kế tiếp.
        /// </summary>
        /// <param name="currentCombination">Tổ hợp k phẩn tử.</param>
        /// <param name="k">Số lượng phần tử mỗi tổ hợp.</param>
        /// <param name="elements">Danh sách các phần tử.</param>
        /// <returns></returns>
        public static List<ClassGroupModel> GenNext(List<ClassGroupModel> currentCombination, int k, List<ClassGroupModel> elements)
        {
            int i = k - 1;
            while (i > 0 && currentCombination[i].Equals(elements[elements.Count + i - k]))
                i -= 1;
            ClassGroupModel value = currentCombination[i];
            int index = elements.IndexOf(value);
            currentCombination[i] = elements[index + 1];
            if (i >= 0)
            {
                for (int j = i; j < elements.Count; ++j)
                {
                    if (j >= (currentCombination.Count - 1))
                        break;
                    ClassGroupModel valueTemp = currentCombination[j];
                    int indexTemp = elements.IndexOf(valueTemp);
                    currentCombination[j + 1] = elements[indexTemp + 1];
                }

            }
            return currentCombination;
        }
        #endregion
    }
}
