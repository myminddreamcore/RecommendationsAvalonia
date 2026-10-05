using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Education
{
    public int IdEducation { get; set; }

    public string? NameEducation { get; set; }

    public string? TypeEductaion { get; set; }

    public DateTime? DateStart { get; set; }

    public DateTime? DateEnd { get; set; }

    public string? Result { get; set; }

    public int? IdUser { get; set; }
[JsonIgnore]
    public virtual User? IdUserNavigation { get; set; }
}
