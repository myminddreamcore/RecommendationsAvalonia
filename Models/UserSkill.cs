using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class UserSkill
{
    public int Id { get; set; }

    public int? IdSkill { get; set; }

    public int? MarkSkill { get; set; }

    public int? UserId { get; set; }

    public string? Status { get; set; }
[JsonIgnore]
    public virtual Skill? IdSkillNavigation { get; set; }
[JsonIgnore]
    public virtual User? User { get; set; }
}
