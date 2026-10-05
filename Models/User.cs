using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RecommendationsAvalonia.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? NameUser { get; set; }

    public string? SurnameUser { get; set; }

    public string? PatronymicUser { get; set; }

    public string? PhoneUser { get; set; }

    public string? EmailUser { get; set; }

    public string? PasswordUser { get; set; }

    public string? RoleUser { get; set; }

    public DateTime? DateCreate { get; set; }
[JsonIgnore]
    public virtual ICollection<Confirmation> ConfirmationUserFromNavigations { get; set; } = new List<Confirmation>();
[JsonIgnore]
    public virtual ICollection<Confirmation> ConfirmationUserToNavigations { get; set; } = new List<Confirmation>();
[JsonIgnore]
    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();
[JsonIgnore]
    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
[JsonIgnore]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
[JsonIgnore]
    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
[JsonIgnore]
    public virtual ICollection<UserCompilation> UserCompilations { get; set; } = new List<UserCompilation>();
[JsonIgnore]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
[JsonIgnore]
    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
