using Microsoft.Maui.Storage;
using System.ComponentModel;

namespace TPApp
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        private bool _appVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool AppVisible
        {
            get => _appVisible;
            set
            {
                if (_appVisible != value)
                {
                    _appVisible = value;
                    OnPropertyChanged(nameof(AppVisible));

                    // Update flyout visibility when property changes
                    FlyoutBehavior = value ? FlyoutBehavior.Flyout : FlyoutBehavior.Disabled;
                }
            }
        }

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this;
            Routing.RegisterRoute(nameof(AddFoodItemPage), typeof(AddFoodItemPage));

            // Initialize visibility based on login state
            InitializeVisibility();
        }

        private async void InitializeVisibility()
        {
            try
            {
                var isLoggedIn = await SecureStorage.GetAsync("isLoggedIn");
                AppVisible = isLoggedIn == "true";
            }
            catch
            {
                AppVisible = false;
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                // Clear all auth-related storage
                SecureStorage.Remove("isLoggedIn");
                SecureStorage.Remove("authToken");
                SecureStorage.Remove("userId");

                AppVisible = false;
                await Shell.Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Logout Error", $"Failed to logout: {ex.Message}", "OK");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}