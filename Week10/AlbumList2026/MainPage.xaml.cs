namespace AlbumList
{
    public partial class MainPage : ContentPage
    {
        private AlbumViewModel _viewmodel;
        // Each MainPage instance is one library tab, created with its own url and library number
        public MainPage(string url, int library) {
            InitializeComponent();
            _viewmodel = new AlbumViewModel(url, library);
            BindingContext = _viewmodel;
        }

        // Navigates to the detail page for the tapped album
        private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (e.CurrentSelection.FirstOrDefault() is Album selectedAlbum) {

                // These are received by the [QueryProperty] attribute on AlbumDetailPage
                var parameters = new Dictionary<string, object>
                {
                    { "album", selectedAlbum }
                };
                // Clearing the selection afterwards lets the same album be tapped again
                await Shell.Current.GoToAsync(nameof(AlbumDetailPage), parameters); ((CollectionView)sender).SelectedItem = null;
            }
        }

        // Opens the edit page with only the view model, so it is treated as adding a new album
        private async void AddAlbumButton_Clicked(object sender, EventArgs e) {
            var parameters = new Dictionary<string, object>
    {
        { "vm", _viewmodel }
    };
            await Shell.Current.GoToAsync(nameof(AlbumEditPage), parameters);
        }

        // Opens the edit page with both the view model and the album to edit
        private async void EditButton_Clicked(object sender, EventArgs e) {
            // The button's binding context is the Album from the item template
            if (((Button)sender).BindingContext is Album album) {
                var parameters = new Dictionary<string, object>
                {
                    { "vm", _viewmodel },
                    { "album", album }
                };
                await Shell.Current.GoToAsync(nameof(AlbumEditPage), parameters);
            }
        }

        protected override async void OnAppearing() {
            base.OnAppearing();
            // Give the view model the tab for this page so it can update the tab title
            _viewmodel.LibraryTab = Shell.Current?.CurrentItem?.CurrentItem?.CurrentItem;

            // Only load the albums the first time the page appears
            if (!_viewmodel.IsLoaded) {
                await _viewmodel.DownloadAlbums();
            }
        }
    }
}
