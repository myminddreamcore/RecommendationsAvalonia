using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Text;
using System.Net.Http;
using RecommendationsAvalonia.Models;
using Newtonsoft.Json;
using System.Linq;
using Avalonia.Threading;
using System.Collections.Generic;

namespace RecommendationsAvalonia;

public partial class Main : Window
{
    private DispatcherTimer? _timer;
    private readonly HttpClient _client = new HttpClient();
    public User current;

    public Main(User user)
    {
        InitializeComponent();

        var fioBlock = this.FindControl<TextBlock>("fio");
        current = user;
        if (fioBlock != null)
        {
            fioBlock.Text = $"{user.NameUser} {user.SurnameUser} {user.PatronymicUser}";
        }

        var roleBlock = this.FindControl<TextBlock>("role");
        if (roleBlock != null)
        {
            roleBlock.Text = user.RoleUser;
        }

        Load();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(300)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        Load();
    }
    public void Button_Click1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainWindow m = new MainWindow();
        m.Show();
        Close();
    }
    public async void Load()
    {
        try
        {
            string url = "http://localhost:5000/dashbord/summary/1";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var d = JsonConvert.DeserializeObject<Dashbord>(content);

                if (d != null)
                {
                    var profilBlock = this.FindControl<TextBlock>("profil");
                    if (profilBlock != null)
                    {
                        profilBlock.Text = $"Профили: {d.ColProfils}\n+{d.ProfilsPlus} за неделю";
                    }

                    var vacancyBlock = this.FindControl<TextBlock>("vacancy");
                    if (vacancyBlock != null)
                    {
                        vacancyBlock.Text = $"Вакансии: {d.Vacancy}\n+{d.VacancyProzent} за неделю";
                    }

                    var ratingBlock = this.FindControl<TextBlock>("rating");
                    if (ratingBlock != null)
                    {
                        ratingBlock.Text = $"Рейтинг: {d.Rating}";
                    }
                }
            }

            await Load2();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка Load: {ex.Message}");
        }
    }

    public async System.Threading.Tasks.Task Load2()
    {
        try
        {
            string url = "http://localhost:5000/skills/top";
            var response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var d = JsonConvert.DeserializeObject<List<ratingDTO>>(content);

                var skillsControl = this.FindControl<TextBlock>("skills");

                if (skillsControl != null && d != null && d.Count > 0)
                {
                    string items = "";

                    foreach (var item in d)
                    {
                        if (item?.r != null)
                        {
                            items += $"{item.Name} {item.r.LevelRating} ";

                            int col = 0;
                            for (int i = 0; i < item.r.LevelRating / 10; i++)
                            {
                                items += "█";
                                col++;
                            }
                            for (int i = col; i < 10; i++)
                            {
                                items += "░";
                            }

                            items += "\n";
                        }
                    }

                    skillsControl.Text = items;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка Load2: {ex.Message}");
        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Load();
    }

    private void TextBlock_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var search = new Search(current);
        search.Show();
        Close();
    }

    private void TextBlock_PointerPressed_1(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var podborka = new Podborka(current);
        podborka.Show();
        Close();
    }

    private void TextBlock_PointerPressed_2(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var main = new Main(current);
        main.Show();
        Close();
    }
}