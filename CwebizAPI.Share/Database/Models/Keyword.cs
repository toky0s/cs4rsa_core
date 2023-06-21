using System;
using System.Collections.Generic;

namespace CwebizAPI.Share.Database.Models;

public partial class Keyword
{
    public int Id { get; set; }

    public string Keyword1 { get; set; } = null!;

    public int CourseId { get; set; }

    public string SubjectName { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string? Cache { get; set; }

    public int? DisciplineId { get; set; }

    public virtual Discipline? Discipline { get; set; }
}
