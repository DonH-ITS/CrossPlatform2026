namespace AlbumList;

// Shell passes the selected Album in through the "album" query parameter,
// which is assigned to the album property below
[QueryProperty(nameof(album), "album")]
public partial class AlbumDetailPage : ContentPage
{
    // Remembers the current layout (null until the first size is known)
    private bool? _isWide;
    private bool _isDownloading = false;
    // Raises a change for IsNotDownloading too, since it is derived from this value
    public bool IsDownloading
    {
        get => _isDownloading;
        set
        {
            _isDownloading = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotDownloading));
        }
    }

    public bool IsNotDownloading => !_isDownloading;

    public Album album { get; set; }
    public AlbumDetailPage() {
        InitializeComponent();

    }

    private async void Button_Clicked(object sender, EventArgs e) {
        await Shell.Current.GoToAsync("..");
    }

    // The album is set by Shell before the page appears, so the binding
    // context is assigned here rather than in the constructor
    protected override async void OnAppearing() {
        base.OnAppearing();
        BindingContext = album;
        IsDownloading = true;
        try {
            var img = await App.Http.GetByteArrayAsync(album.CoverUrl);
            ImageDisplay.Source = ImageSource.FromStream(() => new MemoryStream(img));
        }
        catch {

        }
        IsDownloading = false;

    }

    // Switches between a side-by-side layout (wide) and a stacked layout (narrow)
    protected override void OnSizeAllocated(double width, double height) {
        base.OnSizeAllocated(width, height);

        bool isWide = width > 700;
        // Only rebuild the grid when the layout actually needs to change
        if (isWide == _isWide) {
            return;
        }
        _isWide = isWide;

        MainGrid.RowDefinitions.Clear();
        MainGrid.ColumnDefinitions.Clear();

        if (isWide) {
            // Details on the left, tracks on the right (twice as wide)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2, GridUnitType.Star)));
            Grid.SetRow(DetailsStack, 0);
            Grid.SetColumn(DetailsStack, 0);
            Grid.SetRow(TracksGrid, 0);
            Grid.SetColumn(TracksGrid, 1);
        }
        else {
            // Details on top, tracks below filling the remaining space
            MainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            MainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            Grid.SetRow(DetailsStack, 0);
            Grid.SetColumn(DetailsStack, 0);
            Grid.SetRow(TracksGrid, 1);
            Grid.SetColumn(TracksGrid, 0);
        }
    }
}