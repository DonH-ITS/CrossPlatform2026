namespace AlbumList;

// Shell fills these properties from the parameters passed to GoToAsync.
// album is only supplied when editing, so it is null when adding.
[QueryProperty(nameof(album), "album")]
[QueryProperty(nameof(viewModel), "vm")]
public partial class AlbumEditPage : ContentPage
{
    public Album album { get; set; }
    public AlbumViewModel viewModel { get; set; }
    // True when editing an existing album, false when adding a new one
    private bool _isUpdating = false;
    public AlbumEditPage() {
        InitializeComponent();
    }

    // Builds an Album from the form fields and adds it or replaces the existing one
    private async void Save_Clicked(object sender, EventArgs e) {
        // Year is the only field that needs validating before saving
        if (!int.TryParse(YearEntry.Text, out int year)) {
            await DisplayAlertAsync("Error", "Year must be number", "OK");
            return;
        }
        var newAlbum = new Album
        {
            Title = TitleEntry.Text,
            Artist = ArtistEntry.Text,
            Year = year,
            Genre = GenreEntry.Text,
            CoverUrl = CoverEntry.Text,
            // Split the editor text into one track per line, dropping blank lines and extra spaces
            Tracks = (TracksEditor.Text ?? "").Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList()
        };
        if (_isUpdating) {
            viewModel.ReplaceAlbum(album, newAlbum);
        }
        else {
            viewModel.AddAlbum(newAlbum);
        }
        await Shell.Current.GoToAsync("..");
    }

    private async void Cancel_Clicked(object sender, EventArgs e) {
        await Shell.Current.GoToAsync("..");
    }

    // The query properties are set before the page appears, so this decides
    // whether the page is adding or editing and fills in the form if editing
    protected override void OnAppearing() {
        base.OnAppearing();
        if (album == null)
            Title = "Add Album";
        else {
            Title = "Edit Album";
            _isUpdating = true;
            TitleEntry.Text = album.Title;
            ArtistEntry.Text = album.Artist;
            YearEntry.Text = album.Year.ToString();
            GenreEntry.Text = album.Genre;
            CoverEntry.Text = album.CoverUrl;
            // The editor shows one track per line
            TracksEditor.Text = string.Join("\n", album.Tracks);
        }
    }
}
