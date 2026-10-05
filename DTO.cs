using System;
using System.Collections.Generic;
using RecommendationsAvalonia.Models;
namespace RecommendationsAvalonia;
public class UserDTO
{
    public User? user {get;set;}
    public string? token {get;set;}
}
public class Dashbord
{
    public double? ColProfils {get;set;}
    public double? ProfilsPlus {get;set;}
    public double? Vacancy {get;set;}
    public double? VacancyProzent {get;set;}
    public double? Rating {get;set;}
}
public class ratingDTO
{
    public Rating? r {get;set;}
    public string? Name {get;set;}
}
public class FinderDTO
{
    public int? Id {get;set;}
    public string? Name {get;set;}
    public List<string>? Skills {get;set;}
    public double? Rating {get;set;}
}
public class Find
{
    public List<string>? skills {get;set;}
    public double? UrovenOt {get;set;}
    public double? UrovenODo {get;set;}
    public double? OpytOt {get;set;}
    public double? OpytDo {get;set;}
    public double? RatingOt {get;set;}
    public double? ratingDo {get;set;}
}
public class FinderrDTO
{
    public int Id { get; set; }     
    public string Name { get; set; } 
    public string Skills { get; set; } 
    public double Rating { get; set; } 
}
public class Skillss
{
    public int? Id {get;set;}
    public string? Name {get;set;}
    public double? Rating {get;set;}
    public int? Apply {get;set;}
     public DateTime? Date {get;set;}
}
public class PodborDTO
{
    public int? Id {get;set;}
    public string? Name {get;set;}
     public DateTime? Date {get;set;}
     public List<FinderrDTO> users {get;set;}
}