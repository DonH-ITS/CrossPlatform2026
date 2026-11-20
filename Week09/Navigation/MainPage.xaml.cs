namespace Week9Demo
{
    public partial class MainPage : ContentPage
    {
        private readonly AppSettings _settings = App.Settings;
        private string[] _names = { "Mercury", "Venus", "Earth", "Mars" };
        private int[] _moons = { 0, 0, 1, 2 };
        public MainPage(int whichLibrary) {
            InitializeComponent();
            MessageLabel.Text = "whichLibrary";
        }

        public MainPage() {
            InitializeComponent();
        }

        private async void Planet_Clicked(object? sender, EventArgs e) {
            Button button = (Button)sender;
            string name = button.Text;
            int moons = _moons[Array.IndexOf(_names, name)];
            await Shell.Current.GoToAsync($"details?name={Uri.EscapeDataString(name)}");
        }
        private async void Settings_Clicked(object? sender, EventArgs e) {
            await Shell.Current.GoToAsync("settings");
        }

        protected override void OnAppearing() {
            base.OnAppearing();
            BackgroundColor = App.Settings.BackgroundColour;
            MessageLabel.FontSize = App.Settings.FontSize;
            SoundLabel.Text = App.Settings.SoundEnabled ? "Sound on" : "Sound off";

        }

        protected override void OnDisappearing() {
            base.OnDisappearing();
        }
    }
}
