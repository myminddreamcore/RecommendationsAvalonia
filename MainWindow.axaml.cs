using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Net;using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net.Http;
namespace RecommendationsAvalonia;

public partial class MainWindow : Window
{
    public bool IsLoggedIn {get;set;}
    public int IsAnswer {get;set;} = 0;
    public HttpClient client;
    public MainWindow()
    {
        InitializeComponent();
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        client = new HttpClient(httpClientHandler);

    }
   
       
   
    public async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
{
    if(email.Text=="" || password.Text==""){
                        await MessageBoxManager.GetMessageBoxStandard("", "введите логин и пароль", ButtonEnum.Ok).ShowAsync();
                        return;

    }
    
    string url = $"http://localhost:5000/User/auth/login/{email.Text}/{HashPassword( password.Text)}";
    

    try
    {
        var response = await client.PostAsync(url, null);

        var responseBody = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            IsLoggedIn = true;
            IsAnswer = 1;

            UserDTO dto = JsonConvert.DeserializeObject<UserDTO>(responseBody);
            if(dto.user.RoleUser=="admin"){
                Main m = new Main(dto.user);
                m.Show();
                this.Close();
            }
            else{
                await MessageBoxManager.GetMessageBoxStandard("", "у вас нет доступа", ButtonEnum.Ok).ShowAsync();
            }
            
        }
        else
        {
            IsAnswer = 1;
            IsLoggedIn = false;
            await MessageBoxManager.GetMessageBoxStandard("уведомление", "неверный емайл или пароль", ButtonEnum.Ok).ShowAsync();
            
        }
    }
    catch (TaskCanceledException)
    {
        IsLoggedIn = false;
        await MessageBoxManager.GetMessageBoxStandard("уведомление", "таймаут запроса", ButtonEnum.Ok).ShowAsync();
        
    }
    catch (Exception ex)
    {
        IsLoggedIn = false;
        await MessageBoxManager.GetMessageBoxStandard("уведомление", $"ошибка: {ex.Message}", ButtonEnum.Ok).ShowAsync();
        
    }

}

    public string HashPassword(string password)
    {
        using(var md5 = MD5.Create()){
            byte[] inputbytes = Encoding.UTF8.GetBytes(password);
            byte[] hashbytes = md5.ComputeHash(inputbytes);
            return BitConverter.ToString(hashbytes).Replace("-", "").ToLower();
        }
    }
}