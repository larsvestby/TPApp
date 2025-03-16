using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Maui.Storage;

namespace TPApp
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        public ObservableCollection<Fridge> FridgeList { get; set; }

        public MainPage(HttpClient httpClient)
        {
            InitializeComponent();
            _httpClient = httpClient;
            FridgeList = new ObservableCollection<Fridge>();
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadFridgesAsync();
        }

        private async void OnAddFoodItemClicked(object sender, EventArgs e)
        {
            var fridgeId = (int)((Button)sender).CommandParameter;
            await Shell.Current.GoToAsync($"{nameof(AddFoodItemPage)}?fridgeId={fridgeId}");
        }

        private async void OnDeleteFoodItemClicked(object sender, EventArgs e)
        {
            try
            {
                var foodItemId = (int)((Button)sender).CommandParameter;
                bool confirm = await DisplayAlert("Confirm",
                    "Delete this food item permanently?", "Yes", "No");

                if (!confirm) return;

                var response = await _httpClient.DeleteAsync($"api/Fooditems/{foodItemId}");

                if (response.IsSuccessStatusCode)
                {
                    await LoadFridgesAsync();
                    await DisplayAlert("Success", "Food item deleted", "OK");
                }
                else
                {
                    await DisplayAlert("Error", $"Failed to delete: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async Task LoadFridgesAsync()
        {
            try
            {
                var userId = await SecureStorage.GetAsync("userId");
                if (!int.TryParse(userId, out int userIdInt))
                {
                    await DisplayAlert("Error", "User not logged in.", "OK");
                    return;
                }

                var response = await _httpClient.GetAsync($"api/Fridges/user/{userIdInt}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var content = await response.Content.ReadAsStringAsync();
                    var fridges = JsonSerializer.Deserialize<List<Fridge>>(content, options);

                    // Load food items for each fridge
                    foreach (var fridge in fridges)
                    {
                        var fridgeDetailResponse = await _httpClient.GetAsync($"api/Fridges/{fridge.FridgeId}");
                        if (fridgeDetailResponse.IsSuccessStatusCode)
                        {
                            var fridgeContent = await fridgeDetailResponse.Content.ReadAsStringAsync();
                            var fullFridge = JsonSerializer.Deserialize<Fridge>(fridgeContent, options);
                            fridge.FoodItems = fullFridge?.FoodItems;
                        }
                    }

                    FridgeList.Clear();
                    foreach (var fridge in fridges)
                    {
                        FridgeList.Add(fridge);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnAddFridgeClicked(object sender, EventArgs e)
        {
            try
            {
                var fridgeName = await DisplayPromptAsync("New Fridge", "Enter fridge name:");
                if (string.IsNullOrWhiteSpace(fridgeName)) return;

                var userId = await SecureStorage.GetAsync("userId");
                if (!int.TryParse(userId, out int userIdInt))
                {
                    await DisplayAlert("Error", "User not logged in.", "OK");
                    return;
                }

                var fridgeData = new { FridgeName = fridgeName };
                var json = JsonSerializer.Serialize(fridgeData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/Fridges/user/{userIdInt}", content);

                if (response.IsSuccessStatusCode)
                {
                    await LoadFridgesAsync();
                    await DisplayAlert("Success", "Fridge created successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", $"Failed to create fridge: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnDeleteFridgeClicked(object sender, EventArgs e)
        {
            try
            {
                var fridgeId = (int)((Button)sender).CommandParameter;
                bool confirm = await DisplayAlert("Confirm",
                    "Delete this fridge and all its contents?", "Yes", "No");

                if (!confirm) return;

                var response = await _httpClient.DeleteAsync($"api/Fridges/{fridgeId}");

                if (response.IsSuccessStatusCode)
                {
                    await LoadFridgesAsync();
                    await DisplayAlert("Success", "Fridge deleted successfully!", "OK");
                }
                else
                {
                    await DisplayAlert("Error", $"Failed to delete fridge: {response.StatusCode}", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    public class Fridge
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("fridgeId")]
        public int FridgeId { get; set; }

        [JsonPropertyName("fridgeName")]
        public string FridgeName { get; set; }

        [JsonPropertyName("fooditems")]
        public List<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
    }

    public class FoodItem
    {
        [JsonPropertyName("foodItemId")]
        public int FoodItemId { get; set; }

        [JsonPropertyName("foodItemName")]
        public string FoodItemName { get; set; }

        [JsonPropertyName("foodCategory")]
        public int FoodCategory { get; set; }

        [JsonPropertyName("foodItemQuantity")]
        public float FoodItemQuantity { get; set; }

        [JsonPropertyName("foodItemMeasurementUnit")]
        public int FoodItemMeasurementUnit { get; set; }

        [JsonPropertyName("foodItemExpirationDate")]
        public DateTime FoodItemExpirationDate { get; set; }
    }
}