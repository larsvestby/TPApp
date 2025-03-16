using Microsoft.Maui.Storage;
using System.Text;
using System.Net.Http;

namespace TPApp;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient; // Injected client

    // Modified constructor to accept HttpClient
    public LoginPage(HttpClient httpClient)
    {
        InitializeComponent();
        _httpClient = httpClient; // Store the injected client
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            string isLoggedIn = await SecureStorage.GetAsync("isLoggedIn");
            if (isLoggedIn == "true")
            {
                await Shell.Current.GoToAsync("//MainPage");
                var appShell = (AppShell)Shell.Current;
                appShell.AppVisible = true;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Startup error: {ex.Message}", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegistrationPage");
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        if (UsernameEntry == null || PasswordEntry == null)
        {
            await DisplayAlert("Error", "UI elements not initialized.", "OK");
            return;
        }

        string username = UsernameEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please enter both fields.", "OK");
            return;
        }

        try
        {
            // Use the injected HttpClient instead of creating a new one
            var loginData = new { Username = username, Password = password };
            var json = System.Text.Json.JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Changed to use relative path (base address is already configured)
            HttpResponseMessage response = await _httpClient.PostAsync("api/Users/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseString);

                await SecureStorage.SetAsync("authToken", result.token);
                await SecureStorage.SetAsync("userId", result.userId.ToString());
                await SecureStorage.SetAsync("isLoggedIn", "true");

                await Shell.Current.GoToAsync("//MainPage");
                var appShell = (AppShell)Shell.Current;
                appShell.AppVisible = true;
            }
            else
            {
                await DisplayAlert("Error", "Invalid credentials (code: " + response.StatusCode + ")", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Connection Error",
                $"Failed to connect: {ex.GetType().Name} - {ex.Message}",
                "OK");
        }
    }

    public class LoginResponse
    {
        public string token { get; set; }
        public int userId { get; set; }
        public string username { get; set; }
    }
}