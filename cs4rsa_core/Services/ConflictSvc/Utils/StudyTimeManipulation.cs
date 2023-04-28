using Cs4rsa.Services.ConflictSvc.DataTypes;
using Cs4rsa.Services.SubjectCrawlerSvc.DataTypes;

using System;
using System.Collections.Generic;

namespace Cs4rsa.Services.ConflictSvc.Utils
{
    /// <summary>
    /// Bao gồm các phương thức thao tác với StudyTime.
    /// </summary>
    public class StudyTimeManipulation
    {
        /// <summary>
        /// Trả về khoảng thời gian giao nhau giữa hai StudyTime, 
        /// nếu chúng không giao nhau trả về null.
        /// </summary>
        /// <param name="studyTime1">StudyTime.</param>
        /// <param name="studyTime2">StudyTime.</param>
        /// <returns>
        /// StudyTimeIntersect đại diện cho một 
        /// khoảng giao về thời gian giữa hai StudyTime.
        /// Phục vụ cho việc phát hiện xung đột.
        /// </returns>
        public static StudyTimeIntersect GetStudyTimeIntersect(StudyTime studyTime1, StudyTime studyTime2)
        {
            List<DateTime> studyTimes = new()
            {
                studyTime1.Start,
                studyTime1.End,
                studyTime2.Start,
                studyTime2.End
            };
            studyTimes.Sort();
            if (studyTimes[1] == studyTime2.Start
                && studyTimes[2] == studyTime1.End
                && studyTime2.Start < studyTime1.End)
            {
                return new StudyTimeIntersect(studyTime2.Start, studyTime1.End, studyTime1, studyTime2);
            }
            if (studyTimes[1] == studyTime1.Start
                && studyTimes[2] == studyTime2.End
                && studyTime1.Start < studyTime2.End)
            {
                return new StudyTimeIntersect(studyTime1.Start, studyTime2.End, studyTime1, studyTime2);
            }
            if (studyTimes[0] == studyTime1.Start && studyTimes[3] == studyTime1.End)
            {
                return new StudyTimeIntersect(studyTime2.Start, studyTime2.End, studyTime1, studyTime2);
            }
            if (studyTimes[0] == studyTime2.Start && studyTimes[3] == studyTime2.End)
            {
                return new StudyTimeIntersect(studyTime1.Start, studyTime1.End, studyTime1, studyTime2); ;
            }
            if (studyTimes[0] == studyTime2.Start
                && studyTimes[2] == studyTime2.End
                && studyTime2.End < studyTime1.End)
            {
                return new StudyTimeIntersect(studyTime2.Start, studyTime2.End, studyTime1, studyTime2);
            }
            if (studyTimes[0] == studyTime1.Start
                && studyTimes[2] == studyTime1.End
                && studyTime1.End < studyTime2.End)
            {
                return new StudyTimeIntersect(studyTime1.Start, studyTime1.End, studyTime1, studyTime2);
            }
            return StudyTimeIntersect.Instance;
        }

        /// <summary>
        /// Bắt cặp các StudyTime trong cùng một List.
        /// </summary>
        /// <param name="studyTimes">List các StudyTime.</param>
        /// <returns>List các Tuple là cặp các StudyTime.</returns>
        public static IEnumerable<Tuple<StudyTime, StudyTime>> PairStudyTimes(List<StudyTime> studyTimes)
        {
            int index = 0;
            while (index < studyTimes.Count - 1)
            {
                StudyTime firstItem = studyTimes[index];
                for (int j = index + 1; j <= studyTimes.Count - 1; ++j)
                {
                    yield return new(firstItem, studyTimes[j]);
                }
                index++;
            }
        }

        /// <summary>
        /// Lấy ra các StudyTimeIntersect từ List các Tuple StudyTime.
        /// </summary>
        /// <param name="studyTimeTuples">List các Tuple StudyTime.</param>
        /// <returns>Danh sách các StudyTimeIntersect.</returns>
        public static IEnumerable<StudyTimeIntersect> GetStudyTimeIntersects(IEnumerable<Tuple<StudyTime, StudyTime>> studyTimeTuples)
        {
            foreach (Tuple<StudyTime, StudyTime> item in studyTimeTuples)
            {
                StudyTimeIntersect studyTimeIntersect = GetStudyTimeIntersect(item.Item1, item.Item2);
                if (!studyTimeIntersect.Equals(StudyTimeIntersect.Instance))
                {
                    yield return studyTimeIntersect;
                }
            }
        }
    }
}
