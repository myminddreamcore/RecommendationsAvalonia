using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Compilation
{
    public int IdCompilation { get; set; }

    public string? NameCompilation { get; set; }

    public DateTime? Date { get; set; }
[JsonIgnore]
    public virtual ICollection<UserCompilation> UserCompilations { get; set; } = new List<UserCompilation>();
}
