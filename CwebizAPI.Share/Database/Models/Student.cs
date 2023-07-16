using System;
using System.Collections.Generic;

namespace CwebizAPI.Share.Database.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string SpecialString { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime? BirthDay { get; set; }

    public string? Cmnd { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? AvatarImgPath { get; set; }

    public int? CurriculumId { get; set; }

    public virtual Curriculum? Curriculum { get; set; }
}
