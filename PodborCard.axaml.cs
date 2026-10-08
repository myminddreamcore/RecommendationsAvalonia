using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using RecommendationsAvalonia.Models;
namespace RecommendationsAvalonia;

public partial class PodborCard : Window
{
    public PodborCard(int? id)
    {
        InitializeComponent();
        curr = id;
        Load(id);

    }
    public int? curr = 0;
    public HttpClient client = new HttpClient();
    public async void Load(int? id)
    {
        string url = $"http://localhost:5000/Compilation/GetAll";

        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            List<string> all = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());
            cb.ItemsSource = all;
        }
    }
    private async void add_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (cb.SelectedItem == null)
        {
            return;
        }
        string url = $"http://localhost:5000/Compilation/AddNew/{curr}/{cb.SelectedItem.ToString()}";

        var response = await client.PostAsync(url, null);
        if (response.IsSuccessStatusCode)
        {
            await MessageBoxManager.GetMessageBoxStandard("", "успешно", ButtonEnum.Ok).ShowAsync();
            Close();

        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard("", "пользователь уже есть в этой подборке", ButtonEnum.Ok).ShowAsync();

        }
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}