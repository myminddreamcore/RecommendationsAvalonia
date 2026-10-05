using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Rating
{
    public int IdRating { get; set; }

    public DateTime? DateRating { get; set; }

    public string? TypeRating { get; set; }

    public double? LevelRating { get; set; }

    public int? UserId { get; set; }

    public int? IdSkill { get; set; }
[JsonIgnore]
    public virtual Skill? IdSkillNavigation { get; set; }
[JsonIgnore]
    public virtual User? User { get; set; }
}
