using System.Text;
using System.Text.Json;
using Microsoft.Maui.Controls;

namespace TPApp;

[QueryProperty(nameof(FridgeId), "fridgeId")]
public partial class AddFoodItemPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private int _fridgeId;

    // MAUI Shell query property for fridgeId
    public string FridgeId { get; set; }  // Shell will automatically populate this

    public AddFoodItemPage(HttpClient httpClient)
    {
        InitializeComponent();
        _httpClient = httpClient;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Parse the fridgeId from query parameter
        if (!int.TryParse(FridgeId, out _fridgeId))
        {
            DisplayAlert("Error", "Invalid Fridge ID", "OK").ContinueWith(_ => Navigation.PopAsync());
        }
    }

    // Removed manual query parsing methods

    private async void OnAddFoodItemClicked(object sender, EventArgs e)
    {
        // Validation checks remain the same
        if (string.IsNullOrWhiteSpace(FoodNameEntry.Text) ||
            CategoryPicker.SelectedIndex == -1 ||
            string.IsNullOrWhiteSpace(QuantityEntry.Text) ||
            UnitPicker.SelectedIndex == -1)
        {
            await DisplayAlert("Error", "Please fill all fields.", "OK");
            return;
        }

        if (!float.TryParse(QuantityEntry.Text, out float quantity))
        {
            await DisplayAlert("Error", "Invalid quantity.", "OK");
            return;
        }

        try
        {
            // Food item creation remains the same
            var foodItem = new
            {
                FoodCategory = CategoryPicker.SelectedIndex,
                FoodItemName = FoodNameEntry.Text,
                FoodItemQuantity = quantity,
                FoodItemMeasurementUnit = UnitPicker.SelectedIndex,
                FoodItemExpirationDate = ExpirationDatePicker.Date.ToString("yyyy-MM-dd")
            };

            var json = JsonSerializer.Serialize(foodItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // API call to create food item
            var response = await _httpClient.PostAsync("api/Fooditems", content);
            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", "Failed to create food item.", "OK");
                return;
            }

            // Link to fridge - now uses properly parsed _fridgeId
            var responseContent = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdItem = JsonSerializer.Deserialize<FoodItemResponse>(responseContent, options);

            var linkData = new { FoodItemId = createdItem.foodItemId };
            var linkJson = JsonSerializer.Serialize(linkData);
            var linkContent = new StringContent(linkJson, Encoding.UTF8, "application/json");

            var linkResponse = await _httpClient.PostAsync($"api/Fridges/{_fridgeId}/Fooditems", linkContent);

            if (!linkResponse.IsSuccessStatusCode)
                await DisplayAlert("Error", "Failed to add item to fridge.", "OK");
            else
            {
                await DisplayAlert("Success", "Food item added!", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private class FoodItemResponse
    {
        public int foodItemId { get; set; }
    }
}
