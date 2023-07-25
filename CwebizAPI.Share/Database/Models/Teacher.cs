using System;
using System.Collections.Generic;

namespace CwebizAPI.Share.Database.Models;

public partial class Teacher
{
    public string TeacherId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Sex { get; set; }

    public string? Place { get; set; }

    public string? Degree { get; set; }

    public string? WorkUnit { get; set; }

    public string? Position { get; set; }

    public string? Subject { get; set; }

    public string? Form { get; set; }
}
