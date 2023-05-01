using Cs4rsa.Constants;
using Cs4rsa.Cs4rsaDatabase.Models;

using System.Collections.Generic;

namespace Cs4rsa.Services.TeacherCrawlerSvc.Models
{
    public class TeacherModel
    {
        public int TeacherId { get; }
        public string Name { get; }
        public string Sex { get; }
        public string Place { get; }
        public string Degree { get; }
        public string WorkUnit { get; }
        public string Position { get; }
        public string Subject { get; }
        public string Form { get; }
        public IEnumerable<string> TeachedSubjects { get; set; }
        public string Path { get; }
        public string Url { get; }

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
                TeachedSubjects = teacher.TeachedSubjects.Split(VmConstants.SeparatorTeacherSubject);
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
