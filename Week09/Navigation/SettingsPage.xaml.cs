namespace Week9Demo;

public partial class SettingsPage : ContentPage
{
    private AppSettings _copy = App.Settings.Clone();
    public SettingsPage()
	{
		InitializeComponent();
        BindingContext = _copy;
    }

    private void Colour_Clicked(object sender, EventArgs e) {
        Button button = (Button)sender;
        _copy.BackgroundColour = button.BackgroundColor ?? Colors.White;
    }

    private async void Save_Clicked(object sender, EventArgs e) {
        App.Settings.CopyFrom(_copy);
        await Shell.Current.GoToAsync("..");
    }

    private async void Cancel_Clicked(object sender, EventArgs e) {
        await Shell.Current.GoToAsync("..");
    }
}