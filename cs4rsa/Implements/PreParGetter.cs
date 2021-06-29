using cs4rsa.BasicData;
using cs4rsa.Crawler;
using cs4rsa.Interfaces;
using System.Collections.Generic;

namespace cs4rsa.Implements
{
    public class PreParGetter : IGetter<PreParSubject>
    {
        public PreParSubject Get(object[] parameters = null)
        {
            string sessionId = (string)parameters[0];
            string courseId = (string)parameters[1];
            DtuSubjectCrawler crawler = new DtuSubjectCrawler(sessionId, courseId);
            List<string> prerequisiteSubjects = crawler.PrerequisiteSubjects;
            List<string> parallelSubjects = crawler.ParallelSubjects;
            return new PreParSubject(prerequisiteSubjects, parallelSubjects);
        }
    }
}
