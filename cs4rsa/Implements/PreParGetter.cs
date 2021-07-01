using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Database;
using cs4rsa.Interfaces;
using System.Collections.Generic;

namespace cs4rsa.Implements
{
    public class PreParGetter : IGetter<PreParSubject>
    {
        /// <summary>
        /// Trả về một đối tượng chứa mã môn tiên quyết và môn song hành của một một nào đó.
        /// </summary>
        /// <param name="parameters">Nếu mảng có hai tham số, tham số đầu là sessionId, thứ hai là courseId
        /// của môn học cần tìm. Nếu mảng có một tham số, tham số đó sẽ là courseId của môn cần tìm.</param>
        /// <returns></returns>
        public PreParSubject Get(object[] parameters = null)
        {
            if (parameters.Length == 2)
            {
                string sessionId = (string)parameters[0];
                string courseId = (string)parameters[1];
                DtuSubjectCrawler crawler = new DtuSubjectCrawler(sessionId, courseId);
                List<string> prerequisiteSubjects = crawler.PrerequisiteSubjects;
                List<string> parallelSubjects = crawler.ParallelSubjects;
                return new PreParSubject(prerequisiteSubjects, parallelSubjects);
            }
            else
            {
                string courseId = (string)parameters[0];
                List<string> preSubjects = Cs4rsaDataView.GetPreSubjects(courseId);
                List<string> parSubjects = Cs4rsaDataView.GetParSubjects(courseId);
                return new PreParSubject(preSubjects, parSubjects);
            }
        }
    }
}
