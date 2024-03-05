﻿using System.Collections.Generic;

namespace Cs4rsa.Database.Models
{
    public class DbProgramSubject
    {
        public int DbProgramSubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string CourseId { get; set; }
        public string Name { get; set; }
        public int Credit { get; set; }

        public List<ParProDetail> ParProDetails { get; set; }
        public List<PreProDetail> PreProDetails { get; set; }
        public int CurriculumId { get; set; }
        public Curriculum Curriculum { get; set; }
    }
}
