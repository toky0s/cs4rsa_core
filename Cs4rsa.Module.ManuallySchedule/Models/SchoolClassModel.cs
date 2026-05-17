using Cs4rsa.Service.SubjectCrawler.DataTypes;
using Cs4rsa.Service.SubjectCrawler.DataTypes.Enums;

namespace Cs4rsa.Module.ManuallySchedule.Models
{
    public class SchoolClassModel
    {
        //public string Color { get; set; }
        public string SubjectCode { get; set; }
        public string SchoolClassName { get; set; }
        //public string SubjectName { get; set; }
        public string RegisterCode { get; set; }
        //public string EmptySeat { get; set; }
        //public string RegistrationTermEnd { get; set; }
        //public string RegistrationTermStart { get; set; }
        //public string RegistrationStatus { get; set; }
        //public string ImplementationStatus { get; set; }
        public ClassForm Type { get; set; }
        public DayPlaceMetadata DayPlaceMetaData { get; set; }
        public Schedule Schedule { get; set; }
        public SchoolClass SchoolClass { get; }
        public ClassGroupModel ClassGroupModel { get; set; }
        public StudyWeek StudyWeek { get; set; }
        //public IEnumerable<string> Rooms { get; set; }
        //public IEnumerable<string> TempTeachers { get; set; }
        //public IEnumerable<Place> Places { get; set; }

        /// <summary>
        /// Trả về <see cref="SchoolClass.CurrentPhase"/> của lần
        /// tính toán Phase gần nhất.
        /// </summary>
        public Phase Phase { get => SchoolClass.CurrentPhase; }

        public SchoolClassModel(SchoolClass schoolClass, ClassGroupModel classGroupModel)
        {
            SchoolClass           = schoolClass;
            SchoolClassName       = schoolClass.SchoolClassName;
            RegisterCode          = schoolClass.RegisterCode;
            Type                  = schoolClass.Type;
            //EmptySeat             = schoolClass.EmptySeat;
            //RegistrationTermEnd   = schoolClass.RegistrationTermEnd;
            //RegistrationTermStart = schoolClass.RegistrationTermStart;
            StudyWeek             = schoolClass.StudyWeek;
            Schedule              = schoolClass.Schedule;
            SubjectCode           = schoolClass.Subject.SubjectCode;
            //Rooms                 = schoolClass.Rooms;
            //Places                = schoolClass.Places;
            //TempTeachers          = schoolClass.TeacherNames;
            //RegistrationStatus    = schoolClass.RegistrationStatus;
            //ImplementationStatus  = schoolClass.ImplementationStatus;
            DayPlaceMetaData      = schoolClass.DayPlaceMetaData; 
            //SubjectName           = schoolClass.SubjectName;
            ClassGroupModel       = classGroupModel;
        }
    }
}
