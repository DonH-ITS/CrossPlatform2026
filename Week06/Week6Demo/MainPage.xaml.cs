using System.Text.Json;
using Microsoft.Maui.Devices;

namespace Week6Demo;

public partial class MainPage : ContentPage
{
    List<Person> people;
    string fileName;

    int appearingCount;
    int disappearingCount;
    int sizeChangedCount;
    int displayChangedCount;

    // Created once, not every time we save
    private static readonly JsonSerializerOptions jsonOptions =
        new JsonSerializerOptions { WriteIndented = true };

    public MainPage() {
        InitializeComponent();

        people = new List<Person>();
        fileName = Path.Combine(FileSystem.Current.AppDataDirectory, "people.json");

        SizeChanged += OnPageSizeChanged;

        System.Diagnostics.Debug.WriteLine("Constructor");
        System.Diagnostics.Debug.WriteLine($"Width in constructor: {Width}"); // -1, not laid out yet

        ConstructorLabel.Text = $"Constructor: ran once, Width at that point was {Width}";
    }

    // ---------- Page lifecycle ----------

    protected override void OnAppearing() {
        base.OnAppearing();
        System.Diagnostics.Debug.WriteLine("OnAppearing");

        DeviceDisplay.Current.MainDisplayInfoChanged += OnMainDisplayInfoChanged;

        appearingCount++;
        AppearingLabel.Text = $"OnAppearing: called {appearingCount} time(s), last at {DateTime.Now:T}, Width then was {Width}";
        SubscribedLabel.Text = "Subscribed to MainDisplayInfoChanged: true";

        UpdateDisplayLabels(DeviceDisplay.Current.MainDisplayInfo);
    }

    protected override void OnDisappearing() {
        DeviceDisplay.Current.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;

        base.OnDisappearing();
        System.Diagnostics.Debug.WriteLine("OnDisappearing");

        // Only visible when the page is shown again
        disappearingCount++;
        DisappearingLabel.Text = $"OnDisappearing: called {disappearingCount} time(s), last at {DateTime.Now:T}";
        SubscribedLabel.Text = "Subscribed to MainDisplayInfoChanged: false";
    }

    // ---------- Lists ----------

    private void OnAddClicked(object? sender, EventArgs e) {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || !int.TryParse(AgeEntry.Text, out int age)) {
            StatusLabel.Text = "Status: enter a name and a whole-number age";
            return;
        }

        people.Add(new Person
        {
            Name = NameEntry.Text,
            Age = age
        });

        NameEntry.Text = "";
        AgeEntry.Text = "";
        StatusLabel.Text = "Status: added";

        ShowPeople();
    }

    private void OnRemoveClicked(object? sender, EventArgs e) {
        // RemoveAll is safe, unlike calling Remove inside a foreach
        int removed = people.RemoveAll(p => p.Name == NameEntry.Text);

        StatusLabel.Text = $"Status: removed {removed}";

        ShowPeople();
    }

    private void OnHighlightClicked(object? sender, EventArgs e) {
        foreach (var item in PeopleLayout.Children) {
            // Children holds IView objects, so check the type first
            if (item is Label label) {
                label.TextColor = Colors.Red;
            }
        }
    }

    private void ShowPeople() {
        PeopleLayout.Clear();

        foreach (var person in people) {
            var label = new Label
            {
                Text = $"{person.Name} ({person.Age})"
            };

            PeopleLayout.Add(label);
        }

        CountLabel.Text = $"Count: {people.Count}";
    }

    // ---------- Files and JSON ----------

    private async void OnSaveClicked(object? sender, EventArgs e) {
        try {
            string json = JsonSerializer.Serialize(people, jsonOptions);
            await File.WriteAllTextAsync(fileName, json);

            JsonLabel.Text = json;
            StatusLabel.Text = "Status: saved";
        }
        catch (Exception ex) {
            StatusLabel.Text = "Status: save failed";
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    private async void OnLoadClicked(object? sender, EventArgs e) {
        if (!File.Exists(fileName)) {
            StatusLabel.Text = "Status: no saved file yet";
            return;
        }

        try {
            string json = await File.ReadAllTextAsync(fileName);
            List<Person>? loaded = JsonSerializer.Deserialize<List<Person>>(json);

            if (loaded != null) {
                people = loaded;
            }

            JsonLabel.Text = json;
            StatusLabel.Text = "Status: loaded";
            ShowPeople();
        }
        catch (Exception ex) {
            StatusLabel.Text = "Status: load failed";
            System.Diagnostics.Debug.WriteLine(ex.Message);
        }
    }

    // ---------- Page size and display information ----------

    // Fires when the page's allocated size changes (resizing a Windows window does this)
    private void OnPageSizeChanged(object? sender, EventArgs e) {
        sizeChangedCount++;
        SizeChangedCountLabel.Text = $"SizeChanged raised: {sizeChangedCount} times";
        PageSizeLabel.Text = $"Page: {Width:F0} x {Height:F0} (layout units)";
        LastEventLabel.Text = $"Last event: SizeChanged at {DateTime.Now:T}";
    }

    // Fires when the display changes (rotation, resolution, scaling), not when a window is resized
    private void OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e) {
        displayChangedCount++;
        DisplayChangedCountLabel.Text = $"MainDisplayInfoChanged raised: {displayChangedCount} times";
        UpdateDisplayLabels(e.DisplayInfo);
        LastEventLabel.Text = $"Last event: MainDisplayInfoChanged at {DateTime.Now:T}";
    }

    private void UpdateDisplayLabels(DisplayInfo display) {
        DisplayPixelsLabel.Text = $"Display pixels: {display.Width} x {display.Height}";
        DensityLabel.Text = $"Density: {display.Density}";
        UnitsLabel.Text = $"Display in layout units: {display.Width / display.Density:F0} x {display.Height / display.Density:F0}";
        OrientationLabel.Text = $"Orientation: {display.Orientation}";
    }
}

public class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
}
