﻿using System.Collections.Generic;

namespace cs4rsa_core.Services.SubjectCrawlerSvc.DataTypes
{
    /// <summary>
    /// Đối tượng chứa thông tin các môn tiên quyết và xong hành
    /// </summary>
    public class PreParContainer
    {
        public List<string> PrerequisiteSubjects { get; set; }
        public List<string> ParallelSubjects { get; set; }
    }
}