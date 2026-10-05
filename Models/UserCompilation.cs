using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class UserCompilation
{
    public int IdUserCompilation { get; set; }

    public int? IdCompilation { get; set; }

    public int? IdUser { get; set; }
[JsonIgnore]
    public virtual Compilation? IdCompilationNavigation { get; set; }
[JsonIgnore]
    public virtual User? IdUserNavigation { get; set; }
}
