using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AlbumList
{
    // ViewModel for one album library (one tab). INotifyPropertyChanged lets the UI
    // update automatically when a bound property changes.
    public class AlbumViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Album> _albums = new();
        private string _libraryName = "";
        private bool _isLoading = false;
        private string _errorMessage = "";
        private string _url = "";
        private int _libraryNo = 0;
        public Command<string> SortCommand { get; }

        // The Shell tab for this library, so its title can follow the library name
        public ShellContent LibraryTab { get; set; }

        public ObservableCollection<Album> Albums
        {
            get => _albums;
            set
            {
                if (_albums != value) {
                    _albums = value;
                    OnPropertyChanged();
                }
            }
        }

        // Changing the name also saves it to Preferences and updates the tab title
        public string LibraryName
        {
            get => _libraryName;
            set
            {
                if (_libraryName != value) {
                    _libraryName = value;
                    Preferences.Default.Set($"library{_libraryNo}", _libraryName);
                    // LibraryTab is not set yet when the constructor assigns the name
                    if (LibraryTab != null)
                        LibraryTab.Title = _libraryName;
                    OnPropertyChanged();
                }
            }
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (_errorMessage != value) {
                    _errorMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (_isLoading != value) {
                    _isLoading = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsLoaded { get; private set; }

        // The number identifies the library and is used for its Preferences key and cache file
        public AlbumViewModel(string url, int number) {
            _url = url;
            _libraryNo = number;
            LibraryName = Preferences.Default.Get($"library{_libraryNo}", $"My Library {_libraryNo}");
            SortCommand = new Command<string>(SortAlbums);
            IsLoaded = false;
        }

        // Loads the albums from the local cache if there is one, otherwise from the web
        public async Task DownloadAlbums() {
            IsLoading = true;
            ErrorMessage = "";
            try {
                string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, $"library{_libraryNo}.json");
                // If the cached file exists, use it
                // Probably need to check the library name for invalid characters but fine for now
                if (File.Exists(fileName)) {
                    string text = await File.ReadAllTextAsync(fileName);
                    Albums = JsonSerializer.Deserialize<ObservableCollection<Album>>(text);
                }
                else {

                    // Send an HTTP GET request using _httpClient.
                    using var response = await App.Http.GetAsync(_url);
                    // Check that the response was successful.
                    if (response.IsSuccessStatusCode) {
                        string text = await response.Content.ReadAsStringAsync();
                        Albums = JsonSerializer.Deserialize<ObservableCollection<Album>>(text);
                        // Cache the file locally
                        await File.WriteAllTextAsync(fileName, text);

                    }

                }

                IsLoaded = true;

            }
            catch (Exception ex) {
                ErrorMessage = ex.Message;
                return;
            }
            finally {
                // Finally ALWAYS runs after the others
                IsLoading = false;
            }
        }

        // Assigning a new collection (rather than sorting in place) triggers the
        // Albums setter, which is what refreshes the UI
        public void SortAlbums(string sortBy) {
            switch (sortBy) {
                case "Title":
                    Albums = new ObservableCollection<Album>(
                        Albums.OrderBy(a => a.Title));
                    break;

                case "Artist":
                    Albums = new ObservableCollection<Album>(
                        Albums.OrderBy(a => a.Artist));
                    break;

                case "Year":
                    Albums = new ObservableCollection<Album>(
                        Albums.OrderBy(a => a.Year));
                    break;
            }
        }

        // Overwrites the cache file with the current albums
        private async Task SaveLibrary() {
            string fileName = Path.Combine(FileSystem.Current.AppDataDirectory, $"library{_libraryNo}.json");
            string json = JsonSerializer.Serialize(Albums);
            await File.WriteAllTextAsync(fileName, json);
        }

        public async void AddAlbum(Album album) {
            Albums.Add(album);
            await SaveLibrary();
        }

        public async void ReplaceAlbum(Album oldAlbum, Album newAlbum) {
            int index = Albums.IndexOf(oldAlbum);
            if (index >= 0) {
                Albums[index] = newAlbum;
                await SaveLibrary();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // [CallerMemberName] fills in the calling property's name automatically
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
