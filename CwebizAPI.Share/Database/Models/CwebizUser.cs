using System;
using System.Collections.Generic;

namespace CwebizAPI.Share.Database.Models;

public partial class CwebizUser
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int UserType { get; set; }

    public string? StudentId { get; set; }
}
