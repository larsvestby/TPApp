using Microsoft.Maui.Storage;
using System.ComponentModel;  // For INotifyPropertyChanged

namespace TPApp
{
    public partial class AppShell : Shell, INotifyPropertyChanged
    {
        private bool _appVisible;

        // Implement INotifyPropertyChanged to update UI when property changes
        public event PropertyChangedEventHandler PropertyChanged;

        public bool AppVisible
        {
            get => _appVisible;
            set
            {
                if (_appVisible != value)
                {
                    _appVisible = value;
                    OnPropertyChanged(nameof(AppVisible)); // Notify the UI about the change
                }
            }
        }

        public AppShell()
        {
            InitializeComponent();
            BindingContext = this; // Set the BindingContext to the current instance to enable data binding
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                SecureStorage.Remove("isLoggedIn");
                AppVisible = false; // Hide the MainPage in the Flyout after logout
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SecureStorage Error on Logout: {ex.Message}");
                await DisplayAlert("Error", "Failed to clear login state.", "OK");
                return;
            }

            // Navigate to LoginPage after logout
            await Shell.Current.GoToAsync("//LoginPage");
        }

        // To trigger property change notification
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
