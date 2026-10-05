using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Confirmation
{
    public int IdConfirmation { get; set; }

    public int? UserFrom { get; set; }

    public int? UserTo { get; set; }

    public int? IdSkill { get; set; }

    public string? Status { get; set; }

    public DateTime? Date { get; set; }
[JsonIgnore]
    public virtual Skill? IdSkillNavigation { get; set; }
[JsonIgnore]
    public virtual User? UserFromNavigation { get; set; }
[JsonIgnore]
    public virtual User? UserToNavigation { get; set; }
}
