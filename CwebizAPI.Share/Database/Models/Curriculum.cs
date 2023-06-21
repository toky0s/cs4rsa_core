using System;
using System.Collections.Generic;

namespace CwebizAPI.Share.Database.Models;

public partial class Curriculum
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
