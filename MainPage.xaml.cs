using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace TPApp
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<Fridge> FridgeList { get; set; }

        public MainPage()
        {
            InitializeComponent();

            FridgeList = new ObservableCollection<Fridge>
            {
                new Fridge { Name = "Fridge 1", FoodItems = new List<string> { "Milk", "Eggs", "Cheese" } },
                new Fridge { Name = "Fridge 2", FoodItems = new List<string> { "Apples", "Butter", "Yogurt" } },
                new Fridge { Name = "Fridge 3", FoodItems = new List<string> { "Chicken", "Spinach", "Tomatoes" } },
                new Fridge { Name = "Fridge 4", FoodItems = new List<string> { "Juice", "Carrots", "Ice Cream" } },
                new Fridge { Name = "Fridge 5", FoodItems = new List<string> { "Pasta", "Salmon", "Lettuce" } }
            };
            BindingContext = this;
        }
    }

    public class Fridge
    {
        public string Name { get; set; }
        public List<string> FoodItems { get; set; }
    }
}
