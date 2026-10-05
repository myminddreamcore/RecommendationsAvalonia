using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using RecommendationsAvalonia.Models;

namespace RecommendationsAvalonia;

public partial class Card : Window
{
    public int? curr = 0;
    private readonly HttpClient _client = new HttpClient();
    
    public Card(int? id)
    {
        InitializeComponent();
        curr = id;
        Load(id);
    }
    
    private string GetToken()
    {
        try
        {
            string jsonContent = File.ReadAllText("/home/user/Рабочий стол/Recommendations/auth.json");
            var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(jsonContent);
            return jsonObject["token"]?.ToString() ?? "";
        }
        catch
        {
            return "";
        }
    }
    
    private void SetAuthHeader()
    {
        string token = GetToken();
        _client.DefaultRequestHeaders.Clear();
        if (!string.IsNullOrEmpty(token))
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }
    }
    
    public async void Load(int? id)
    {
        try
        {
            string url = $"http://localhost:5000/User/users/{id}?mode=employer";
            SetAuthHeader();
            var response = await _client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(content);
                
                if (user != null)
                {
                    var numberBlock = this.FindControl<TextBlock>("number");
                    if (numberBlock != null)
                    {
                        numberBlock.Text = user.IdUser.ToString();
                    }
                    
                    var fiooBlock = this.FindControl<TextBlock>("fioo");
                    if (fiooBlock != null)
                    {
                        fiooBlock.Text = $"{user.SurnameUser} {user.NameUser} {user.PatronymicUser}";
                    }
                    
                    var fioBlock = this.FindControl<TextBlock>("fio");
                    if (fioBlock != null)
                    {
                        fioBlock.Text = $"{user.SurnameUser} {user.NameUser} {user.PatronymicUser}";
                    }
                }
            }
            
            await Load2(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка Load: {ex.Message}");
        }
    }
    
    public async System.Threading.Tasks.Task Load2(int? id)
    {
        try
        {
            string url = $"http://localhost:5000/users/{id}/rating";
            SetAuthHeader();
            var response = await _client.PostAsync(url, null);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var rating = JsonConvert.DeserializeObject<Rating>(content);
                
                if (rating?.LevelRating != null)
                {
                    double level = rating.LevelRating.Value;
                    var trustBlock = this.FindControl<TextBlock>("trust");
                    
                    if (trustBlock != null)
                    {
                        var sb = new StringBuilder();
                        sb.Append($"Доверие сообщества {level} ");
                        
                        int col = 0;
                        for (int i = 0; i < level / 10; i++)
                        {
                            sb.Append("█");
                            col++;
                        }
                        for (int i = col; i < 10; i++)
                        {
                            sb.Append("░");
                        }
                        
                        trustBlock.Text = sb.ToString();
                    }
                }
            }
            
            await Load3(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка Load2: {ex.Message}");
        }
    }
    
    public async System.Threading.Tasks.Task Load3(int? id)
    {
        try
        {
            string url = $"http://localhost:5000/users/{id}/ratingskills";
            SetAuthHeader();
            var response = await _client.PostAsync(url, null);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                double userRating = JsonConvert.DeserializeObject<double>(content);
                
                var competBlock = this.FindControl<TextBlock>("compet");
                
                if (competBlock != null)
                {
                    var sb = new StringBuilder();
                    sb.Append($"Индекс компетенций {userRating} ");
                    
                    int col = 0;
                    for (int i = 0; i < userRating / 10; i++)
                    {
                        sb.Append("█");
                        col++;
                    }
                    for (int i = col; i < 10; i++)
                    {
                        sb.Append("░");
                    }
                    
                    competBlock.Text = sb.ToString();
                }
            }
            
            await Load4(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка Load3: {ex.Message}");
        }
    }
    
    public async System.Threading.Tasks.Task Load4(int? id)
{
    try
    {
        string url = $"http://localhost:5000/Skill/skillsuser/{id}";
        SetAuthHeader();
        var response = await _client.GetAsync(url);
        
        var skillsControl = this.FindControl<ItemsControl>("skills");
        
        if (response.IsSuccessStatusCode && skillsControl != null)
        {
            var content = await response.Content.ReadAsStringAsync();
            var userSkills = JsonConvert.DeserializeObject<List<Skillss>>(content);
            
            if (userSkills != null)
            {
                var items = new List<string>();
                
                foreach (var item in userSkills)
                {
                    var sb = new StringBuilder();
                    sb.Append($"{item.Name} {item.Rating}/10 ");
                    for (int i = 0; i < item.Apply; i++)
                    {
                        sb.Append("*");
                    }
                    
                    if (item.Date.HasValue)
                    {
                        sb.Append($" (подтверждено: {item.Date.Value.ToString("dd.MM.yyyy")})");
                    }
                    
                    items.Add(sb.ToString());
                }
                
                skillsControl.ItemsSource = items;
            }
        }
        
        await Load5(id);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка Load4: {ex.Message}");
    }
}

public async System.Threading.Tasks.Task Load5(int? id)
{
    try
    {
        string url = $"http://localhost:5000/Skill/worksuser/{id}";
        SetAuthHeader();
        var response = await _client.GetAsync(url);
        
        var workControl = this.FindControl<ItemsControl>("work");
        
        if (response.IsSuccessStatusCode && workControl != null)
        {
            var content = await response.Content.ReadAsStringAsync();
            var userWork = JsonConvert.DeserializeObject<List<Experience>>(content);
            
            if (userWork != null)
            {
                var items = new List<string>();
                
                foreach (var item in userWork)
                {
                    string dateStart = item.DateStart?.ToString("dd.MM.yyyy") ?? "";
                    string dateEnd = item.DateEnd?.ToString("dd.MM.yyyy") ?? "настоящее";
                    string h = $"{dateStart} — {dateEnd} | {item.NameCompany} | {item.Type}";
                    
                    if (item.DateEnd == null)
                    {
                        h = $"★ {h}";
                    }
                    
                    items.Add(h);
                }
                
                workControl.ItemsSource = items;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка Load5: {ex.Message}");
    }
}
    
    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
    
    private async void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await MessageBoxManager.GetMessageBoxStandard("Информация", "Функция в разработке", ButtonEnum.Ok).ShowAsync();
    }
    
    private async void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var podborCard = new PodborCard(curr);
        podborCard.Show();
    }
}