using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class UserRole
{
    public int IdUserRole { get; set; }

    public int? IdUser { get; set; }

    public string? NameRoleBefore { get; set; }

    public string? NameRoleAfter { get; set; }
[JsonIgnore]
    public virtual User? IdUserNavigation { get; set; }
}
