using cs4rsa_core.Constants;
using cs4rsa_core.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace cs4rsa_core.Services.TeacherCrawlerSvc.Models
{
    public class TeacherModel
    {
        public int TeacherId { get; init; }
        public string Name { get; init; }
        public string Sex { get; init; }
        public string Place { get; init; }
        public string Degree { get; init; }
        public string WorkUnit { get; init; }
        public string Position { get; init; }
        public string Subject { get; init; }
        public string Form { get; init; }
        public IEnumerable<string> TeachedSubjects { get; set; }
        public string Path { get; init; }
        public string Url { get; init; }

        public TeacherModel(Teacher teacher)
        {
            TeacherId = teacher.TeacherId;
            Name = teacher.Name;
            Sex = teacher.Sex;
            Place = teacher.Place;
            Degree = teacher.Degree;
            WorkUnit = teacher.WorkUnit;
            Position = teacher.Position;
            Subject = teacher.Subject;
            Form = teacher.Form;
            if (string.IsNullOrEmpty(teacher.TeachedSubjects))
            {
                TeachedSubjects = new List<string>();
            }
            else
            {
                TeachedSubjects = teacher.TeachedSubjects.Split(VMConstants.SPRT_TEACHER_SUBJECTS);
            }
            Path = teacher.Path;
            Url = teacher.Url;
        }

        public TeacherModel(
            int teacherId, 
            string name, 
            string sex, 
            string place, 
            string degree, 
            string workUnit, 
            string position, 
            string subject, 
            string form, 
            IEnumerable<string> teachedSubjects, 
            string path,
            string url)
        {
            TeacherId = teacherId;
            Name = name;
            Sex = sex;
            Place = place;
            Degree = degree;
            WorkUnit = workUnit;
            Position = position;
            Subject = subject;
            Form = form;
            TeachedSubjects = teachedSubjects;
            Path = path;
            Url = url;
        }

        public TeacherModel(int teacherId, string name)
        {
            TeacherId = teacherId;
            Name = name;
        }
    }
}
