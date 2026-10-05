using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Skill
{
    public int IdSkill { get; set; }

    public string? NameSkill { get; set; }
[JsonIgnore]
    public virtual ICollection<Confirmation> Confirmations { get; set; } = new List<Confirmation>();
[JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
[JsonIgnore]
    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
