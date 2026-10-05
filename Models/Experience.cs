using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Experience
{
    public int Id { get; set; }

    public string? Type { get; set; }

    public string? NameCompany { get; set; }

    public string? Post { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public int? IdUser { get; set; }
[JsonIgnore]
    public virtual User? IdUserNavigation { get; set; }
}
