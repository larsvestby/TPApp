using Microsoft.Maui.Storage;
using System.Text;

namespace TPApp;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Get the value securely and check if it's null
            string isLoggedIn = await SecureStorage.GetAsync("isLoggedIn");

            if (isLoggedIn == null)
            {
                Console.WriteLine("No login state found.");
                return;
            }

            if (isLoggedIn == "true")
            {
                // Navigate to MainPage if already logged in
                await Shell.Current.GoToAsync("//MainPage");
                var appShell = (AppShell)Shell.Current;
                appShell.AppVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SecureStorage Error: {ex.Message}");
        }
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        if (UsernameEntry == null || PasswordEntry == null)
        {
            await DisplayAlert("Error", "UI elements are not properly initialized.", "OK");
            return;
        }

        string username = UsernameEntry.Text?.Trim();
        string password = PasswordEntry.Text?.Trim();

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please enter both username and password.", "OK");
            return;
        }

        var loginData = new { Username = username, Password = password };
        string apiUrl = "https://localhost:7226/api/Users/login"; // Change to your API URL

        using (var client = new HttpClient())
        {
            var json = System.Text.Json.JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var result = System.Text.Json.JsonSerializer.Deserialize<LoginResponse>(responseString);

                    // Store login token securely
                    await SecureStorage.SetAsync("authToken", result.Token);
                    await SecureStorage.SetAsync("userId", result.UserId.ToString());

                    // Navigate to MainPage
                    await Shell.Current.GoToAsync("//MainPage");
                    var appShell = (AppShell)Shell.Current;
                    appShell.AppVisible = true;
                }
                else
                {
                    await DisplayAlert("Login Failed", "Invalid username or password.", "OK");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login Error: {ex.Message}");
                await DisplayAlert("Error", "Failed to connect to server.", "OK");
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }



    private bool AuthenticateUser(string username, string password)
    {
        return username == "test" && password == "1234"; // Replace with real authentication
    }
}
