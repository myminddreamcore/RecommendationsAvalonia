using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class Notification
{
    public int IdNotification { get; set; }

    public int? IdUser { get; set; }

    public string? Description { get; set; }

    public DateTime? Date { get; set; }
[JsonIgnore]
    public virtual User? IdUserNavigation { get; set; }
}
