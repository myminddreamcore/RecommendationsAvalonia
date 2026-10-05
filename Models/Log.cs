using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Log
{
    public int IdLog { get; set; }

    public string? TypeLog { get; set; }

    public string? DescriptionLog { get; set; }

    public int? LogId { get; set; }
}
