using System;
using System.Collections.Generic;

namespace CwebizAPI.Share.Database.Models;

public partial class Course
{
    public int Id { get; set; }

    public string? YearInfor { get; set; }

    public string? YearValue { get; set; }

    public string? SemesterInfor { get; set; }

    public string? SemesterValue { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ICollection<Discipline> Disciplines { get; set; } = new List<Discipline>();
}
