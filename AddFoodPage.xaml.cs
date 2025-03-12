using System.Collections.ObjectModel;

namespace TPApp;

public enum FoodCategory
{
    Dairy,
    Meat,
    Vegetables,
    Fruits,
    Grains,
    Beverages,
    Snacks,
    Seafood,
    Other
}

public enum MeasurementUnit
{
    Grams,
    Kilograms,
    Liters,
    Milliliters,
    Pieces
}

public partial class AddFoodPage : ContentPage
{
    public ObservableCollection<string>? FoodCategories { get; set; }
    public ObservableCollection<string>? MeasurementUnits { get; set; }

    public string? SelectedFoodCategory { get; set; }
    public string? SelectedMeasurementUnit { get; set; }

    public AddFoodPage()
    {
        InitializeComponent();
        FoodCategories = [.. Enum.GetNames<FoodCategory>()];
        MeasurementUnits = [.. Enum.GetNames<MeasurementUnit>()];

        // Binding to XAML
        BindingContext = this;
    }
}
