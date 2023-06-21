using System;
using System.Collections.Generic;

namespace CwebizAPI.Share.Database.Models;

public partial class Discipline
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? CourseId { get; set; }

    public virtual Course? Course { get; set; }

    public virtual ICollection<Keyword> Keywords { get; set; } = new List<Keyword>();
}
