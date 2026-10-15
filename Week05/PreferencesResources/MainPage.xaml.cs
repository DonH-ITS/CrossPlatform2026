using System.Diagnostics;

namespace Week5Examples
{
    public partial class MainPage : ContentPage
    {
        private int _highScore;
        private int _gamesPlayed;
        private bool _isLoading = false;
        private bool _soundIconOn = true;
        private readonly Random _random = new Random();

        private readonly string _namesFile =
            Path.Combine(FileSystem.Current.AppDataDirectory, "Names.txt");

        public MainPage() {
            InitializeComponent();

            AspectPicker.ItemsSource = new List<string> { "AspectFit", "AspectFill", "Fill" };
            AspectPicker.SelectedIndex = 0;

            DirLabel.Text = FileSystem.Current.AppDataDirectory;

            LoadPreferences();
            ShowSection(1);
        }

        private void OnPrefTabClicked(object? sender, EventArgs e) {
            ShowSection(1);
        }

        private void OnResTabClicked(object? sender, EventArgs e) {
            ShowSection(2);
        }

        private void OnFileTabClicked(object? sender, EventArgs e) {
            ShowSection(3);
        }

        private void ShowSection(int section) {
            PrefSection.IsVisible = section == 1;
            ResSection.IsVisible = section == 2;
            FileSection.IsVisible = section == 3;
        }

        // Preferences

        private void LoadPreferences() {
            _isLoading = true;

            _highScore = Preferences.Default.Get("HighScore", 0);
            _gamesPlayed = Preferences.Default.Get("GamesPlayed", 0);
            NameEntry.Text = Preferences.Default.Get("PlayerName", "");
            SoundSwitch.IsToggled = Preferences.Default.Get("SoundEnabled", true);

            _isLoading = false;

            UpdateScoreLabels();
        }

        private void UpdateScoreLabels() {
            HighScoreLabel.Text = $"High score: {_highScore}";
            GamesPlayedLabel.Text = $"Games played: {_gamesPlayed}";
        }

        private void OnSaveNameClicked(object? sender, EventArgs e) {
            Preferences.Default.Set("PlayerName", NameEntry.Text ?? "");
            PrefStatusLabel.Text = $"Saved PlayerName → \"{NameEntry.Text}\"";
        }

        private void OnSoundToggled(object? sender, ToggledEventArgs e) {
            if (_isLoading)
                return;

            Preferences.Default.Set("SoundEnabled", e.Value);
            PrefStatusLabel.Text = $"Saved SoundEnabled → {e.Value}";
        }

        private void OnPlayClicked(object? sender, EventArgs e) {
            int score = _random.Next(0, 201);
            LastScoreLabel.Text = $"Last score: {score}";

            _gamesPlayed++;
            Preferences.Default.Set("GamesPlayed", _gamesPlayed);

            if (score > _highScore) {
                _highScore = score;
                Preferences.Default.Set("HighScore", _highScore);
                PrefStatusLabel.Text = $"New high score! Saved HighScore → {_highScore}";
            }
            else {
                PrefStatusLabel.Text = $"{score} did not beat {_highScore}, so HighScore was not saved.";
            }

            UpdateScoreLabels();
        }

        private void OnContainsKeyClicked(object? sender, EventArgs e) {
            bool exists = Preferences.Default.ContainsKey("HighScore");
            PrefStatusLabel.Text = $"ContainsKey(\"HighScore\") → {exists}";
        }

        private void OnRemoveClicked(object? sender, EventArgs e) {
            Preferences.Default.Remove("HighScore");
            _highScore = Preferences.Default.Get("HighScore", 0);
            UpdateScoreLabels();
            PrefStatusLabel.Text = "Removed HighScore. Get() now returns the default (0).";
        }

        private void OnClearClicked(object? sender, EventArgs e) {
            Preferences.Default.Clear();
            LoadPreferences();
            PrefStatusLabel.Text = "Cleared all preferences. Everything is back to its default.";
        }

        // Resources

        private void OnAspectChanged(object? sender, EventArgs e) {
            int index = AspectPicker.SelectedIndex;

            if (index == 0) {
                DemoImage.Aspect = Aspect.AspectFit;
                AspectLabel.Text = "AspectFit: show the whole image";
            }
            else if (index == 1) {
                DemoImage.Aspect = Aspect.AspectFill;
                AspectLabel.Text = "AspectFill: fill the area, possibly cropping";
            }
            else if (index == 2) {
                DemoImage.Aspect = Aspect.Fill;
                AspectLabel.Text = "Fill: stretch the image to the area";
            }
        }

        private void OnToggleIconClicked(object? sender, EventArgs e) {
            _soundIconOn = !_soundIconOn;
            SoundIcon.Text = _soundIconOn ? "\uf028" : "\uf6a9";
        }

        private async void OnReadRawClicked(object? sender, EventArgs e) {
            try {
                using var stream = await FileSystem.OpenAppPackageFileAsync("mydata.txt");
                using var reader = new StreamReader(stream);

                RawLabel.Text = await reader.ReadToEndAsync();
            }
            catch (Exception ex) {
                RawLabel.Text = $"Could not read mydata.txt: {ex.Message}";
            }
        }

        // Files

        private async void OnWriteNamesClicked(object? sender, EventArgs e) {
            try {
                await using StreamWriter writer = new StreamWriter(_namesFile);

                await writer.WriteLineAsync("Chris");
                await writer.WriteLineAsync("Kathryn");
                await writer.WriteLineAsync("Jean");

                FileOutputLabel.Text = "Wrote 3 names to Names.txt";
            }
            catch (Exception ex) {
                FileOutputLabel.Text = $"Error: {ex.Message}";
            }
        }

        private async void OnReadNamesClicked(object? sender, EventArgs e) {
            if (!File.Exists(_namesFile)) {
                FileOutputLabel.Text = "Names.txt does not exist yet. Write it first.";
                return;
            }

            try {
                List<string> names = new List<string>();

                using StreamReader reader = new StreamReader(_namesFile);

                string? line;
                while ((line = await reader.ReadLineAsync()) != null) {
                    names.Add(line);
                }

                FileOutputLabel.Text = $"Read {names.Count} names:\n" + string.Join("\n", names);
            }
            catch (Exception ex) {
                FileOutputLabel.Text = $"Error: {ex.Message}";
            }
        }

        private void OnDeleteFileClicked(object? sender, EventArgs e) {
            try {
                if (File.Exists(_namesFile))
                    File.Delete(_namesFile);

                FileOutputLabel.Text = "Deleted Names.txt (if it existed).";
            }
            catch (Exception ex) {
                FileOutputLabel.Text = $"Error: {ex.Message}";
            }
        }
    }
}