
namespace Week9Demo;

[QueryProperty(nameof(PlanetName), "name")]
[QueryProperty(nameof(MoonCount), "moons")]
public partial class DetailsPage : ContentPage
{
    public string PlanetName { get; set; } = "Not Set Yet";
    public int MoonCount { get; set; } = -1;
    public DetailsPage()
	{
		InitializeComponent();
        // Not Set in Constructor
    }

    private async void Back_Clicked(object sender, EventArgs e) {
        await DisplayAlertAsync("Back Clicked", $"PlanetName: {PlanetName}, MoonCount: {MoonCount}", "OK");
        await Shell.Current.GoToAsync("..");
    }

    protected override bool OnBackButtonPressed() {
        // If you want to intercept pressing the "hardware" back button, this is where
        // Per Microsoft documentation you should return base.OnBackButtonPressed and not 
        // block the button from working at all - that is bad practice for Android/iOS
        return base.OnBackButtonPressed();
    }

    protected override void OnAppearing() {
        //("OnAppe message", $"PlanetName: {PlanetName}", "OK");
        BackgroundColor = App.Settings.BackgroundColour;
       // MessageLabel.FontSize = App.Settings.FontSize;
       // SoundLabel.Text = App.Settings.SoundEnabled ? "Sound on" : "Sound off";
        base.OnAppearing();
    }

}